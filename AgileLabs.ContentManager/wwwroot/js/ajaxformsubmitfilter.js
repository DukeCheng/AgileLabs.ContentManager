$(document).ready(function () {
	$('form').submit(function () {
		// inside event callbacks 'this' is the DOM element so we first 
		// wrap it in a jQuery object and then invoke ajaxSubmit 
		var formFilter = $(this);

		var options = {
			target: formFilter.context.id,   // target element(s) to be updated with server response 
			beforeSubmit: showRequest,  // pre-submit callback 
			success: showResponse,  // post-submit callback 

			// other available options: 
			//url: '',         // override for form's 'action' attribute 
			type: 'post',        // 'get' or 'post', override for form's 'method' attribute 
			//dataType:  null        // 'xml', 'script', or 'json' (expected server response type) 
			//clearForm: true        // clear all form fields after successful submit 
			//resetForm: true        // reset the form after successful submit 
			//data: $('form').formSerialize(),
			//data: $('form').serializeArray(),
			// $.ajax options can be used here too, for example: 
			timeout: 3000
		};
		//options.url = '/submit/FreeTrial/TrialApply/';

		//var data = $('form').serializeArray();

		$(formFilter.context.id).ajaxSubmit(options);

		// 为了防止普通浏览器进行表单提交和产生页面导航 
		//返回false
	    //return false;

	});
});


function showRequest(formData, jqForm, options) {
	var queryString = $.param(formData);

	// jqForm is a jQuery object encapsulating the form element.  To access the 
	// DOM element for the form do this: 
	// var formElement = jqForm[0]; 

	//alert('About to submit: \n\n' + queryString);

	// here we could return false to prevent the form from being submitted; 
	// returning anything other than false will allow the form submit to continue 
	return true;
};

function showResponse(responseText, statusText, xhr, $form) {

}