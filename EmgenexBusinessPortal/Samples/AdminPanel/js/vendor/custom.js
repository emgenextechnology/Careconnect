

// Initialize collapse button
      $(".button-collapse").sideNav();
      // Initialize collapsible (uncomment the line below if you use the dropdown variation)
      //$('.collapsible').collapsible();
      //select option initialization
      $(document).ready(function() {
      $('select').material_select();
      });
// modal Initialization
      $(document).ready(function(){
      // the "href" attribute of .modal-trigger must specify the modal ID that wants to be triggered
      $('.modal').modal();
      });

// Table header sorting
      $(".order").click(function() {
        $(this).toggleClass("active");
      });
// Mobile Filter button
$('.filters-btn').on('click', function(){
  $('.left-window').toggle('show');
});

// Date picker
$('.datepicker').pickadate({
    selectMonths: true, // Creates a dropdown to control month
    selectYears: 15 // Creates a dropdown of 15 years to control year
  });

//slim-scroll

      $('.slim-scroll').slimScroll({
      railVisible: true,
      alwaysVisible: true,
      railOpacity: 0.2,
      height: 'auto'
  });
