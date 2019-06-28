//using System;
//using System.Data;
//using System.Data.Common;
//using System.Threading.Tasks;

//namespace Lxdn.Core.Db
//{
//    public class TransactionRunner<TConnection>
//        where TConnection : DbConnection, new()
//    {
//        private readonly Database<TConnection> database;

//        public TransactionRunner(Database<TConnection> database)
//        {
//            this.database = database;
//        }

//        public async Task Run(Func<ISqlRunner, Task> logic, IsolationLevel isolation = IsolationLevel.Unspecified)
//        {
//            using (var cn = new TConnection { ConnectionString = database.Schema.ConnectionString })
//            {
//                await cn.OpenAsync().ConfigureAwait(false);

//                using (var transaction = cn.BeginTransaction(isolation))
//                {
//                    try
//                    {
//                        await logic(database.RunOn(cn)).ConfigureAwait(false);
//                        transaction.Commit();
//                    }
//                    catch
//                    {
//                        transaction.Rollback();
//                        throw;
//                    }
//                }
//            }


//            //using (var cn = new TConnection { ConnectionString  = schema.ConnectionString })
//            //{
//            //    await cn.OpenAsync().ConfigureAwait(false);

//            //    using (var transaction = cn.BeginTransaction(isolation))
//            //    {
//            //        try
//            //        {
//            //            await logic((TConnection)transaction.Connection).ConfigureAwait(false);
//            //            transaction.Commit();
//            //        }
//            //        catch
//            //        {
//            //            transaction.Rollback();
//            //            throw;
//            //        }
//            //    }
//            //}
//        }
//    }
//}
