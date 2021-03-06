﻿using course_sense_dotnet.Utility;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using Microsoft.Extensions.Configuration;

namespace course_sense_dotnet.DataAccessLayer
{
    public class DataAccess : IDisposable, IDataAccess
    {
        private readonly ILogger logger;
        private readonly IConfiguration configuration;
        private readonly LiteDatabase db;
        private bool disposedValue;

        public DataAccess(ILogger<DataAccess> logger, IConfiguration configuration)
        {
            this.logger = logger;
            this.configuration = configuration;
            db = new LiteDatabase(configuration["LiteDB:ConnectionString"]);
        }
        public bool InsertRequestIntoDB(NotificationRequest request)
        {
            try
            {
                ILiteCollection<NotificationRequest> collection = db.GetCollection<NotificationRequest>("notification_requests");
                collection.Insert(request);
                return true;
            }
            catch (Exception e)
            {
                logger.LogError("Unable to save document into LiteDB: " + e.Message);
                return false;
            }
        }
        public bool RemoveRequestFromDB(NotificationRequest request)
        {
            try
            {
                ILiteCollection<NotificationRequest> collection = db.GetCollection<NotificationRequest>("notification_requests");
                return collection.Delete(request._id);
            }
            catch (Exception e)
            {
                logger.LogError("Unable to delete document from LiteDB: " + e.Message);
                return false;
            }
        }
        public IEnumerable<NotificationRequest> GetCollectionFromDB()
        {
            try
            {
                ILiteCollection<NotificationRequest> collection = db.GetCollection<NotificationRequest>("notification_requests");
                return collection.FindAll();
            }
            catch (Exception e)
            {
                logger.LogError("Could not read from LiteDB: "+e.Message);
                return null;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    db.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
