using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Lxdn.Core.Basics;
using Lxdn.Core.Expressions.Exceptions;
using Lxdn.Core.Expressions.Operators.Models;

namespace Lxdn.Core.Expressions.Operators
{
    public class CodeBlock : Operator
    {
        private readonly Func<Expression> create;

        public CodeBlock(CodeBlockModel model, ExecutionEngine logic)
        {
            this.create = () =>
            {
                var scope = new List<Model>();
                var expressions = new List<Expression>();

                // iterate through all operators in the block. Since all operators return something,
                // it is possible to store the return value (a variable) to be used in subsequent operators:

                // examine if the operator declares such variable (denoted by 'as'-attribute), then
                // store the model for the variable in the current context within the scope of the block
                // (see 'finally') and generate an assignment statement. Otherwise proceed as usual (take 
                // the operator and generate the expression for it). See 'TestRegex' unit test for details.

                // The return value of the block is determined by the last operator in the block.

                try
                {
                    model.Operators.ToList().ForEach(operatorModel =>
                    {
                        var op = logic.Operators.CreateFrom(operatorModel);

                        var let = op as Let;
                        if (let != null)
                        {
                            logic.Models.Add(let.Variable);
                            scope.Add(let.Variable);
                        }

                        expressions.Add(op.Expression);
                    });

                    return Expression.Block(expressions.Last().Type, scope.Select(m => m.AsParameter()), expressions);
                }
                catch (Exception e)
                {
                    throw new ExpressionConfigException("Error in block operator", e);
                }
                finally
                {
                    scope.ForEach(m => logic.Models.Remove(m));
                }
            };
        }

        protected internal override Expression Create()
        {
            return this.create();
        }
    }
}
