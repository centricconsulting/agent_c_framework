$(document).ready(function () {
    $(".splitbutton")
				.click(function () {
				    var menu = $(this).parent().next().show().position({
				        my: "left top",
				        at: "left bottom",
				        of: this
				    });
				    $(document).one("click", function () {
				        menu.hide();
				    });
				    return false;
				})
				.parent()
					.next()
						.hide()
						.menu();
});