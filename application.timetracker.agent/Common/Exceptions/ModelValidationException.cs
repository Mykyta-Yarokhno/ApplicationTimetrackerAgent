using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace application.timetracker.agent.Common.Exceptions
{
    public class ModelValidationException : Exception
    {
        public class ModelInfo
        {
            public string TypeName;
            public string Id;
            public List <ValidationResult> Result;
        }

        private List<ModelInfo> _models;
        public IEnumerable<ModelInfo> Models => _models; 

        public ModelValidationException(string message, IEnumerable<ModelInfo> models)
            :base(message)
        {
            _models = new List<ModelInfo>(models);
        }
    }
}
