using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ITSupport.Business.Results
{
    public class CaseCreationResult
    {
        public bool Success { get; set; }

        public string Message { get; set; }

        public int CaseId { get; set; }
    }
}