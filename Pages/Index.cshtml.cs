using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace App.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime MinDate {get;} = new DateTime(2019,11,07);
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime MaxDate {get;} = new DateTime(2019,11,29);

        public void OnGet()
        {
        }


        // the query is always :
        //    ?minDate=07%2F11%2F2019&maxDate=29%2F11%2F2019
        public void OnGetTestQuery([FromQuery]DateTime minDate, [FromQuery]DateTime maxDate)
        {
            // Now the binded minDate and maxDate are not correct
            //     minDate = 2019-07-11 
            //     maxDate = default
            Debug.Assert(minDate == new DateTime(2019,11,07));    // fail
            Debug.Assert(maxDate == new DateTime(2019,11,29));    // fail

            //------------------------------------------------------
            // parse from the querystring manually
            var converter =  TypeDescriptor.GetConverter(typeof(DateTime));
            var requestCultureFeature = HttpContext.Features.Get<IRequestCultureFeature>();
            var requestCulture = requestCultureFeature.RequestCulture;

            var minStr = HttpContext.Request.Query[nameof(minDate)].First();  
            var minDateValue = (DateTime) converter.ConvertFrom(null, requestCulture.Culture, minStr);  // different

            var maxStr = HttpContext.Request.Query[nameof(maxDate)].First();  
            var maxDateValue = (DateTime) converter.ConvertFrom(null, requestCulture.Culture, maxStr);  // different 
            Debug.Assert(minDateValue == new DateTime(2019,11,07));    // true 
            Debug.Assert(maxDateValue == new DateTime(2019,11,29));    // true
        }
        // the form payload is :
        // minDate=07%2F11%2F2019&maxDate=29%2F11%2F2019
        public void OnPost([FromForm]DateTime minDate, [FromForm]DateTime maxDate)
        {
            // it's fine 
            //     minDate = 2019-11-07
            //     maxDate = 2019-11-29
            Debug.Assert(minDate == new DateTime(2019,11,07));
            Debug.Assert(maxDate == new DateTime(2019,11,29));
        }

    }
}
