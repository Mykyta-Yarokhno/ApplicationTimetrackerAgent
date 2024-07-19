using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace DbUtils
{
    public  static  class DbContextUtils
    {
        public static DbContext ValidateContext(this DbContext db)
        {
            var recordsToValidate =
                                db.ChangeTracker
                                    .Entries()
                                        .Where(entity =>
                                               entity.State == EntityState.Modified
                                            || entity.State == EntityState.Added
                                           );

            foreach (var recordToValidate in recordsToValidate)
            {
                var entity = recordToValidate.Entity;
                var validationContext = new ValidationContext(entity);
                var results = new List<ValidationResult>();

                if (!Validator.TryValidateObject(entity, validationContext, results, true)) // Need to set all properties, otherwise it just checks required.
                {
                    var messages =
                            results
                                .Select(r => r.ErrorMessage)
                                .ToList()
                                .Aggregate((message, nextMessage) => message + ", " + nextMessage);

                    throw new ApplicationException($"Unable to save changes for {entity.GetType().FullName} due to error(s): {messages}");
                }
            }

            return db;
        }
    }
}
