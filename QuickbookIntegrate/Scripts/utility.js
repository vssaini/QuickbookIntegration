'use strict';

const homeController = '/Home/';

var utility = {

    init: () =>
    {
        console.log('Utility initialized.');
    },

    addCustomer: () =>
    {
        $('#status').show();
        const data = {
            Title: 'Mr',
            GivenName: 'Vikram',
            MiddleName: 'Singh',
            FamilyName: 'Saini',
            PrimaryEmailAddr: 'vssaini@gmail.com',
            PrimaryPhone: '9829478688',
            CompanyName: 'Microsoft'
        };

        utility.postAjaxJson(homeController + 'AddCustomer', data, function (response)
        {
            if (response.Status)
            {
                $.notify(response.Message, "success", { autoHideDelay: 5000 });
            } else
            {
                $.notify(response.Message, "error", { autoHideDelay: 10000 });
            }

            $('#status').hide();

        });
    },

    postAjaxJson: (url, data, successCallback) =>
    {
        const json = JSON.stringify(data);
        $.ajax({
            type: 'POST',
            contentType: 'application/json; charset=utf-8',
            url: url,
            data: json,
            success: function (response)
            {
                successCallback(response);
            },
            error: function (xhr)
            {
                const errorMessage = `${xhr.status}:${xhr.statusText}`;
                console.error(errorMessage);
                console.log(xhr.responseText);
            }
        });
    }
};