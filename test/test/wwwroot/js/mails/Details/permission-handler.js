class MailDetailsPermissionHandler {
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
      return response.data.data.modules.find((m) => m.moduleName === "Mails");
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
    const { back, edit, delete: deleteText } = window.mailResources;

    // Back button
    buttons.push(
      this.createLink("/Mails", "fas fa-arrow-left me-1", back, "btn-secondary")
    );

    // Edit button if user has update permission
    if (permissions.canUpdate) {
      buttons.push(
        this.createButton(
          "edit",
          "fas fa-edit me-1",
          edit,
          "editMail()",
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
          "deleteMail()",
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
  window.mailDetailsPermissionHandler = new MailDetailsPermissionHandler();
});
