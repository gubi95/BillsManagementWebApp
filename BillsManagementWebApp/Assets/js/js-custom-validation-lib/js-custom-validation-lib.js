var validationTags = ['not-empty'];
var validationFunctionsNames = ['notEmpty'];
var checkValidationFunctions = {
    notEmpty: function ($field) {
        return $field.val().trim() != '';
    }
};

var valdiationErrorMessages = [
    'To pole nie może być puste!'
];

function Validator(containerSelector) {
    this.fields = [];
    
    var that = this;
    jQuery(containerSelector).find('[data-validators!=""][data-validators]').each(function () {
        that.fields.push(jQuery(this));
    });

    this.validate = function() { return ValidateContainer(this) };
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
                    if (checkValidationFunctions[validationFunctionsNames[k]](container.fields[i]) == false) {
                        container.fields[i].parent().find('.val-err-msg').remove();
                        container.fields[i].parent().append('<span class="val-err-msg" style="color: #54001B; font-family: Lato; font-size: 12px;">' + valdiationErrorMessages[k] + '</span>');

                        //if (container.fields[i].is('input:text') || container.fields[i].is('input:password')) {
                        //    container.fields[i].css('box-shadow', 'inset 0 1px 1px rgba(255,0,0,.075)', 'important');
                        //}

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
            container.fields[i].parent().find('.val-err-msg').remove();
        }
    }

    return isFormValid;
}