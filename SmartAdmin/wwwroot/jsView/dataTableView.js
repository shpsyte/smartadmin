class dataTableSetting{
    tableInit(table, pagelen)
    {
        var objeto = '#' + table;
        var responsiveHelper_datatable_col_reorder = undefined;
        var breakpointDefinition = {
            tablet: 1024,
            phone: 480
        };

        var otable = $(objeto).dataTable({
            "sDom": "t<'dt-toolbar-footer'<'col-sm-6 col-xs-12 hidden-xs'i><'col-xs-12 col-sm-6'p>>",
            "pageLength": pagelen || 10,
            "preDrawCallback": function () {
                // Initialize the responsive datatables helper once.
                if (!responsiveHelper_datatable_col_reorder) {
                    responsiveHelper_datatable_col_reorder = new ResponsiveDatatablesHelper($(objeto), breakpointDefinition);
                }
            },
            "rowCallback": function (nRow) {
                responsiveHelper_datatable_col_reorder.createExpandIcon(nRow);
            },
            "drawCallback": function (oSettings) {
                responsiveHelper_datatable_col_reorder.respond();
            },
        });
    }
}
var tableSetting = new dataTableSetting();