
#if NETFULL

namespace Lxd.Annotations
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using BillGates = System.ComponentModel.DataAnnotations;

    using Lxd.Core.Iteration;
    using Lxd.Core.Extensions;
    
    /// <summary>
    /// Resursively validates an aggregate using Microsoft annotation attributes
    /// </summary>
    public class AnnotationValidator
    {
        private readonly Func<Type, object> resolve;

        public AnnotationValidator(Func<Type, object> resolve = null)
        {
            this.resolve = resolve;
        }

        public IReadOnlyCollection<AnnotationResult> Validate(object payload)
        {
            return this.Validate(ValidationContext.Create(payload
                .ThrowIfDefault(() => new ArgumentNullException(nameof(payload)))));
        }

        internal IReadOnlyCollection<AnnotationResult> Validate(ValidationContext source)
        {
            Func<ValidationContext, IEnumerable<AnnotationResult>> validate = validable =>
            {
                var property = validable.Path.OfType<Step>().Last().Property;

                return property.GetCustomAttributes<ValidationAttribute>(true)
                    .Select(attribute => new
                    {
                        Result = attribute.GetValidationResult(validable.Value, new BillGates.ValidationContext(source.Value) { MemberName = property.Name }),
                        Attribute = attribute
                    })
                    .Where(validation => ValidationResult.Success != validation.Result)
                    .Select(validation => new AnnotationError(validation.Attribute, validation.Result.ErrorMessage))
                    .Select(error => error.For(validable.Path)).ToList().AsReadOnly();
            };

            Func<ValidationContext, IEnumerable<AnnotationResult>> stepInto = validable =>
            {
                var current = validable.Value.GetType();
                var enumerableOf = current.AsArgumentsOf(typeof(IEnumerable<>)).IfHasValue(args => args.Single());

                if (enumerableOf != null && Consider.ForIteration(enumerableOf))
                {
                    var members = (validable.Value as IEnumerable).IfExists(e => e).OfType<object>().ToList();
                    return members.Zip(Enumerable.Range(0, members.Count), (member, index) => validable.StepInto(index))
                        .Select(this.Validate)
                        .SelectMany(results => results);
                }

                return Consider.ForIteration(current) ? this.Validate(validable) : null;
            };

            var ms = new BillGates.ValidationContext(source.Value, new ServiceProvider(resolve), null);

            var errorsFromType = source.Value.IfExists(v => v.GetType()).GetCustomAttributes<CustomValidationAttribute>()
                .Where(rule => rule.GetValidationResult(source.Value, ms) != ValidationResult.Success)
                .Select(rule => new AnnotationError(rule, rule.FormatErrorMessage(source.Value.GetType().Name)))
                .Select(error => error.For(source.Path)).ToList();

            var errors = source.Value.GetType().GetProperties()
                .Where(property => property.HasPublicGetter())
                .Select(source.StepInto)
                .Aggregate((IList<AnnotationResult>)new List<AnnotationResult>(), (validators, validable) =>
                    validators.PushMany(validable.Value.IfExists(value => stepInto(validable)) ?? validate(validable)))
                .Concat(errorsFromType).ToList();

            return errors;
        }
    }
}

#endif