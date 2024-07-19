using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using application.timetracker.agent.Common.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace application.timetracker.agent.Models
{

    public partial class timetrackingagentContext
    {
        public void Validate()
        {
            InternalValidate();
        }

        public override int SaveChanges()
        {
            InternalValidate();

            return base.SaveChanges();
        }

        private void InternalValidate()
        {
            var recordsToValidate =
                    ChangeTracker
                        .Entries()
                            .Where(entity =>
                                   entity.State == Microsoft.EntityFrameworkCore.EntityState.Modified
                                || entity.State == Microsoft.EntityFrameworkCore.EntityState.Added
                               );

            var failedModels = new List<ModelValidationException.ModelInfo>();

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

                    
                }
            }

            if(failedModels.Count > 0)
            {
                // throw new ApplicationException($"Unable to save changes for {entity.GetType().FullName} due to error(s): {messages}");
                throw new ModelValidationException();
            }
        }
    }
}

/*  Modify the validation routine to get all failed entities with their errors
 * 
 * ModelValidationException exception class
  + Models IEnumerable (List)
        < class ModelInfo >
            {
               + TypeName : string
               + Id       : string
               + Result IEnumerable (List) <ValidationResult> <--- LIst of validation errors found for current entity
            }

Print to console all entities that failed their validation  + validation errors for each of entities

*/
