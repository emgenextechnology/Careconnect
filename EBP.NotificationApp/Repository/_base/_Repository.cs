using EBP.NotificationApp.Database;
using EBP.NotificationApp.Entity;
using EBP.NotificationApp.Entity._base;
using EBP.NotificationApp.Response;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBP.NotificationApp.Repository
{
    public class _Repository
    {

        public _Repository()
        {

        }
        private CRMStagingEntities _dBEntity;
        public CRMStagingEntities DBEntity { get { return _dBEntity; } }

        /// <summary>
        /// Initializes DB Connection
        /// </summary>
        public void DBInit()
        {
            this._dBEntity = new CRMStagingEntities();
        }

        /// <summary>
        /// Initializes DB for using statement
        /// </summary>
        /// <returns></returns>
        public CRMStagingEntities DBConnect()
        {
            return new CRMStagingEntities();
        }

        /// <summary>
        /// Closes DB Connection
        /// </summary>
        public void DBClose()
        {
            if (this.DBEntity != null)
                this.DBEntity.Dispose();
        }

        /// <summary>
        /// Saves New Entity
        /// </summary>
        /// <param name="model"></param>
        public int DBSave<T>(T model) where T : class
        {
            int rows = 0;

            try
            {
                DBEntity.Set<T>().Add(model);
                rows = DBEntity.SaveChanges();
            }
            //catch (System.Data.UpdateException ex)
            //{
            //    //DataResponse.ThrowError(ex);
            //}
            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                //DataResponse.ThrowError(e);
                foreach (var eve in e.EntityValidationErrors)
                {
                    //Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                    //    eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        //Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                        //    ve.PropertyName, ve.ErrorMessage);
                        string s = string.Format("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
                    }
                }
            }
            catch (OverflowException e)
            {

            }
            catch (Exception e)
            {
                //DataResponse.ThrowError(e);
            }
            return rows;
        }

        /// <summary>
        /// Updates Entity
        /// </summary>
        /// <param name="model"></param>
        public int DBSaveUpdate<T>(T model) where T : class
        {
            int rows = 0;
            try
            {
                this.DBEntity.Entry<T>(model).State = System.Data.Entity.EntityState.Modified; // .ApplyCurrentValues(this.GetEntity(model), model);
                rows = this.DBEntity.SaveChanges();

            }
            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                //DataResponse.ThrowError(e);
                foreach (var eve in e.EntityValidationErrors)
                {
                    //Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                    //    eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        //Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                        //    ve.PropertyName, ve.ErrorMessage);
                        string s = string.Format("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
                    }
                }
            }
            catch (SqlException ex)
            {
                ex.Log();
            }
            //catch (System.Data.UpdateException e)
            //{
            //    //DataResponse.ThrowError(e);
            //}
            return rows;
        }

        /// <summary>
        /// Deletes Entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int DBDelete<T>(T entity) where T : class
        {
            int rows = 0;
            try
            {
                this.DBEntity.Set<T>().Remove(entity);
                rows = this.DBEntity.SaveChanges();
            }
            catch (SqlException ex)
            {
                ex.Log();
                //DataResponse.ThrowError(ex);
            }
            //catch (System.Data.UpdateException e)
            //{
            //    rows = -1;
            //    //DataResponse.ThrowError(e);
            //}

            return rows;
        }

        public void EntityDelete<T>(T entity) where T : class
        {
            this.DBEntity.Set<T>().Remove(entity);
        }

        public void EntityAdd<T>(T model) where T : class
        {
            this.DBEntity.Set<T>().Add(model);
        }

        public DataResponse<EntityList<T>> GetList<T>(IQueryable<T> query, int skip, int take)
        {
            //int count = query.Count();
            DataResponse<EntityList<T>> response = new DataResponse<EntityList<T>>();

            EntityList<T> entity = new EntityList<T>();
            try
            {
                entity.Pager = new DataPager
                {
                    TotalCount = query.Count(),
                    Skip = skip,
                    Take = take
                };
                entity.List = query.Skip(skip).Take(take).ToList();

                if (entity.List == null)
                    response.CreateResponse(null, DataResponseStatus.NoContent);
                else
                    response.CreateResponse(entity, DataResponseStatus.OK);
            }
            catch (Exception ex)
            {
                //ex.Log();
                response.ThrowError(ex);
            }
            return response;
        }

        public DataResponse<EntityList<T>> GetList<T>(IEnumerable<T> query, int skip, int take)
        {
            //int count = query.Count();
            DataResponse<EntityList<T>> response = new DataResponse<EntityList<T>>();

            EntityList<T> entity = new EntityList<T>();
            try
            {
                entity.Pager = new DataPager
                {
                    TotalCount = query.Count(),
                    Skip = skip,
                    Take = take
                };
                entity.List = query.Skip(skip).Take(take).ToList();

                if (entity.List == null)
                    response.CreateResponse(null, DataResponseStatus.NoContent);
                else
                    response.CreateResponse(entity, DataResponseStatus.OK);
            }
            catch (Exception ex)
            {
                response.ThrowError(ex);
            }

            return response;
        }

        public DataResponse<T> GetFirst<T>(IQueryable<T> query)
        {
            //int count = query.Count();
            DataResponse<T> response = new DataResponse<T>();

            try
            {

                var entity = query.FirstOrDefault();
                response.CreateResponse(entity, DataResponseStatus.OK);
            }

            catch (Exception ex)
            {
                response.ThrowError(ex);
            }
            finally
            {
                //  DBClose();
            }
            return response;
        }

        public void Dispose()
        {

        }
    }
}
