class PermissionHandler {
  constructor() {
    this.checkPermissions();
  }

  // Kiểm tra quyền của người dùng
  async checkPermissions() {
    const token = sessionStorage.getItem("token");
    if (!token) {
      window.location.href = "/Account/Login";
      return;
    }

    try {
      const userId = this.getUserIdFromToken(token);
      const permissions = await this.fetchUserPermissions(userId, token);

      if (permissions) {
        this.renderButtons(permissions);
      }
    } catch (error) {
      console.error("Error checking permissions:", error);
    }
  }

  // Lấy userId từ token
  getUserIdFromToken(token) {
    const payload = JSON.parse(atob(token.split(".")[1]));
    return payload.id;
  }

  // Lấy quyền của user từ API
  async fetchUserPermissions(userId, token) {
    const response = await axios.get(
      `http://localhost:5001/api/UserModules/${userId}`,
      {
        headers: {
          Authorization: `Bearer ${token}`,
        },
      }
    );

    if (response.data.success) {
      return response.data.data.modules.find(
        (m) => m.moduleName === "Notifications"
      );
    }
    return null;
  }

  // Tạo HTML cho từng nút
  createButton(type, icon, text, action, btnClass) {
    return `
      <button class="btn ${btnClass} btn-sm" onclick="${action}">
        <i class="${icon}"></i>${text}
      </button>
    `;
  }

  createLink(href, icon, text, btnClass) {
    return `
      <a href="${href}" class="btn ${btnClass} btn-sm">
        <i class="${icon}"></i>${text}
      </a>
    `;
  }

  // Render các nút dựa trên quyền
  renderButtons(permissions) {
    const buttons = [];
    const { delete: deleteText, newMail: createText } =
      window.notificationResources;

    if (permissions.canDelete) {
      buttons.push(
        this.createButton(
          "delete",
          "bi bi-trash",
          deleteText,
          "deleteSelected()",
          "btn-danger"
        )
      );
    }

    if (permissions.canCreate) {
      buttons.push(
        this.createLink(
          "/Notifications/Create",
          "fas fa-plus me-1",
          createText,
          "btn-success"
        )
      );
    }

    $(".card-header .d-flex.gap-2").html(buttons.join(""));
  }
}

// Khởi tạo khi trang đã sẵn sàng
$(document).ready(() => {
  window.permissionHandler = new PermissionHandler();
});
