function toggleSidebar() {
  $("#sidebar").toggleClass("collapsed");
}

$(document).ready(function () {
  // Xử lý click trên submenu links
  $(".sidebar .collapse .nav-link").on("click", function (e) {
    const parentCollapse = $(this).closest(".collapse");

    if (parentCollapse.length) {
      // Kiểm tra nếu chưa có class show thì thêm vào
      if (!parentCollapse.hasClass("show")) {
        parentCollapse.addClass("show");
      }

      // Đánh dấu link hiện tại là active mà không ảnh hưởng các link khác
      $(this).addClass("active");

      e.stopPropagation();
    }
  });

  // Lưu trạng thái active của dropdown khi load trang
  const currentUrl = window.location.href;
  $(".sidebar .nav-link").each(function () {
    if (this.href === currentUrl) {
      const parentCollapse = $(this).closest(".collapse");
      if (parentCollapse.length) {
        parentCollapse.addClass("show");
      }
      $(this).addClass("active");
    }
  });
});
