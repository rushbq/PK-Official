$(document).ready(function() {
  $("#video").click(function() {
    $(this).css({ color: "#009045", "border-bottom": "2px solid #5db75d" });
    $(".nav-link")
      .not(this)
      .css({ color: "#414141", "border-bottom": "none" });
  });
  $("#feature").click(function() {
    $(this).css({ color: "#009045", "border-bottom": "2px solid #5db75d" });
    $(".nav-link")
      .not(this)
      .css({ color: "#414141", "border-bottom": "none" });
  });
  $("#apply").click(function() {
    $(this).css({ color: "#009045", "border-bottom": "2px solid #5db75d" });
    $(".nav-link")
      .not(this)
      .css({ color: "#414141", "border-bottom": "none" });
  });
  $("#specification").click(function() {
    $(this).css({ color: "#009045", "border-bottom": "2px solid #5db75d" });
    $(".nav-link")
      .not(this)
      .css({ color: "#414141", "border-bottom": "none" });
  });
});
