var validationTags = ['not-empty', 'reg-exp', 'pl-date'];
var validationFunctionsNames = ['notEmpty', 'regExp', 'plDate'];
var checkValidationFunctions = {
    notEmpty: function ($field) {
        return $field.val().trim() != '';
    },
    regExp: function ($field, regularExpression) {        
        return new RegExp(regularExpression).test($field.val());
    },
    plDate: function ($field) {
        var datePLValues = $field.val().split('.').filter(Boolean);        
        return datePLValues.length == 3 && new Date([datePLValues[2], datePLValues[1], datePLValues[0]].join('-')).getTime() > 0;        
    }
};

var valdiationErrorMessages = [
    'To pole nie może być puste!',
    'Wartość pola jest w nieprawidłowym formacie!',
    'Podana data jest nieprawidłowa!'
];

function RemoveFieldFromValidation($field) {
    $field.removeAttr('data-validators');
    $field.closest('div[class*="col-md"]').find('.val-err-msg').remove();
}

function Validator(containerSelector) {
    this.fields = [];
    
    var that = this;
    jQuery(containerSelector).find('[data-validators!=""][data-validators]').each(function () {
        that.fields.push(jQuery(this));
    });

    this.validate = function (includeNewAddedFields) {
        if (arguments.length == 0) {
            return ValidateContainer(this, false);
        }

        if (includeNewAddedFields) {
            that.fields = [];
            jQuery(containerSelector).find('[data-validators!=""][data-validators]').each(function () {
                that.fields.push(jQuery(this));
            });
        }

        return ValidateContainer(this);        
    };
}

function ValidateContainer(container) {
    var isFormValid = true;
    for (var i = 0; i < container.fields.length; i++) {
        var validationAttr = container.fields[i].attr('data-validators') != undefined ? container.fields[i].attr('data-validators') : '';
        var validators = validationAttr.split(',').filter(Boolean);

        var validatorsPassed = 0;
        for (var j = 0; j < validators.length; j++) {

            for (var k = 0; k < validationTags.length; k++) {
                if (validationTags[k] == validators[j]) {
                    var bValidatorPassed = false;
                    // not empty || pl-date
                    if (k == 0 || k == 2) {
                        bValidatorPassed = checkValidationFunctions[validationFunctionsNames[k]](container.fields[i]);
                    }
                    // reg exp
                    else if (k == 1) {
                        var regExp = container.fields[i].attr('data-valid-reg-exp') != undefined ? container.fields[i].attr('data-valid-reg-exp') : '';
                        bValidatorPassed = checkValidationFunctions[validationFunctionsNames[k]](container.fields[i], regExp);
                    }

                    if (!bValidatorPassed) {
                        container.fields[i].closest('div[class*="col-md"]').find('.val-err-msg').remove();
                        container.fields[i].closest('div[class*="col-md"]').append('<span class="val-err-msg" style="color: #54001B; font-family: Lato; font-size: 12px;">' + valdiationErrorMessages[k] + '</span>');
                        isFormValid = false;
                        break;
                    }
                    else {
                        validatorsPassed++;
                    }
                }
            }
        }

        if (validatorsPassed == validators.length) {
            container.fields[i].closest('div[class*="col-md"]').find('.val-err-msg').remove();
        }
    }

    return isFormValid;
}