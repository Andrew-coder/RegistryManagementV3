$(document).ready(function() {
    $("input.daterange").daterangepicker({
        autoUpdateInput: false,
        locale: {
            cancelLabel: 'Clear'
        }});

    $("input.daterange").on('apply.daterangepicker', function(ev, picker) {
        $(this).val(picker.startDate.format('MM/DD/YYYY') + ' - ' + picker.endDate.format('MM/DD/YYYY'));
    });

    $("input.daterange").on('cancel.daterangepicker', function(ev, picker) {
        $(this).val('');
    });
});