function ChangeStatus(e) {
    if ($(e).is(':checked')) {
        $(e).val("1");
    }
    else {
        $(e).val("0");
    }

    console.log($(e).is(':checked'));
}
function DeleteModal(id, url) {
    swal({
        title: "Emin misiniz?",
        text: "Kayıt silinecek, bu işlem geri alınamaz!",
        type: "warning",
        showCancelButton: true,
        confirmButtonColor: "#e69a2a",
        confirmButtonText: "Sil",
        cancelButtonText: "Vazgeç",
        closeOnConfirm: false,
        closeOnCancel: true
    }, function (isConfirm) {
        if (isConfirm) {
            $.post("/" + url + "/Delete/" + id, function (data) {
                if (data.Status === true) {
                    swal("İşlem başarılı!", data.Message, "success");
                    $("#item_" + id).remove();
                }
                else {
                    swal("Hata!", data.Message, "error");
                }
            });

        }
    });
    return false;
}
$("[numberfilter ='true']").keydown(function (e) {
    // Allow: backspace, delete, tab, escape, enter and .
    if ($.inArray(e.keyCode, [46, 8, 9, 27, 13, 110, 188]) !== -1 ||
        // Allow: Ctrl+A, Command+A
        (e.keyCode === 65 && (e.ctrlKey === true || e.metaKey === true)) ||
        // Allow: home, end, left, right, down, up
        (e.keyCode >= 35 && e.keyCode <= 40)) {
        // let it happen, don't do anything
        return;
    }
    // Ensure that it is a number and stop the keypress
    if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
        e.preventDefault();
    }
});
