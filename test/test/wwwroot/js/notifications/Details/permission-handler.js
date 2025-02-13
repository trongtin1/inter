class NotificationDetailsPermissionHandler {
  constructor() {
    this.checkPermissions();
  }

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

  getUserIdFromToken(token) {
    const payload = JSON.parse(atob(token.split(".")[1]));
    return payload.id;
  }

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

  renderButtons(permissions) {
    const buttons = [];
    const {
      back: backText,
      edit: editText,
      delete: deleteText,
    } = window.notificationResources;

    // Back button
    buttons.push(
      this.createLink(
        "/Notifications",
        "fas fa-arrow-left me-1",
        backText,
        "btn-secondary"
      )
    );

    // Edit button if user has update permission
    if (permissions.canUpdate) {
      buttons.push(
        this.createButton(
          "edit",
          "fas fa-edit me-1",
          editText,
          "editNotification()",
          "btn-primary"
        )
      );
    }

    // Delete button if user has delete permission
    if (permissions.canDelete) {
      buttons.push(
        this.createButton(
          "delete",
          "fas fa-trash me-1",
          deleteText,
          "deleteNotification()",
          "btn-danger"
        )
      );
    }

    // Update the buttons container
    $(".card-header .d-flex.gap-2").html(buttons.join(""));
  }
}

// Initialize when document is ready
$(document).ready(() => {
  window.notificationDetailsPermissionHandler =
    new NotificationDetailsPermissionHandler();
});
