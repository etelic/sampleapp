
$("#menu-toggle-2").click(function (e) {
    e.preventDefault();

    $(this).next('span').slideToggle('999999999999');
    $(this).find('span').toggleClass('fa-chevron-left fa-chevron-right');
    $("#wrapper").toggleClass("toggled-2");
    $('#menu ul').hide();
});


