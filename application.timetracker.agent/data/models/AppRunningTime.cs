using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace application.timetracker.agent.Models
{

    //[CustomValidation(typeof(AppRunningTime), "StartFinishDateValidator")]
    public partial class AppRunningTime : IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (this.FinishTime != null)
            {
                if (FinishTime < StartTime)
                {
                    yield
                        return
                            new ValidationResult(
                                "Opps, start date is greater than finish time :)",
                                new[] { "FinishTime", "StartTime" }
                                );
                }
            }
        }

        public static ValidationResult StartFinishDateValidator(AppRunningTime appRT, ValidationContext context)
        {
            if (appRT.FinishTime != null)
            {
                //if (appRT.FinishTime < appRT.StartTime)
                //{
                //    return
                //        new ValidationResult(
                //            "Opps, start date is greater than finish time :)",
                //            new[] { "FinishTime", "StartTime" }
                //            );
                //}
            }

            return ValidationResult.Success;
        }
    }
}
