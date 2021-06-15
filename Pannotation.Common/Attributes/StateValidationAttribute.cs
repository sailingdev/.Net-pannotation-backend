using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Pannotation.Common.Attributes
{
    public class StateValidationAttribute : ValidationAttribute
    {
        private List<string> _availableStates = new List<string>()
        {
            "AK",
            "AL",
            "AR",
            "AS",
            "AZ",
            "CA",
            "CO",
            "CT",
            "DC",
            "DE",
            "FL",
            "FM",
            "GA",
            "GU",
            "HI",
            "IA",
            "ID",
            "IL",
            "IN",
            "KS",
            "KY",
            "LA",
            "MA",
            "MD",
            "ME",
            "MH",
            "MI",
            "MN",
            "MO",
            "MP",
            "MS",
            "MT",
            "NC",
            "ND",
            "NE",
            "NH",
            "NJ",
            "NM",
            "NV",
            "NY",
            "OH",
            "OK",
            "OR",
            "PA",
            "PR",
            "PW",
            "RI",
            "SC",
            "SD",
            "TN",
            "TX",
            "UT",
            "VA",
            "VI",
            "VT",
            "WA",
            "WI",
            "WV",
            "WY"
        };

        public StateValidationAttribute()
        {

        }

        /// <summary>
        /// Returns true if state is emty or valid
        /// </summary>
        public override bool IsValid(object value)
        {
            if (value == null)
                return true;

            if (!(value is string))
                return false;

            var valueString = value as string;

            return _availableStates.Contains(valueString.ToUpper());
        }
    }
}
