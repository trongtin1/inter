async function checkPermissions() {
  const token = localStorage.getItem("token");
  if (!token) {
    window.location.href = "/Account/Login";
    return;
  }

  try {
    // Kiểm tra cache trước
    const cachedModules = sessionStorage.getItem("userModules");
    let permissions;

    if (cachedModules) {
      const modules = JSON.parse(cachedModules);
      permissions = modules.find((m) => m.moduleName === "Notifications");
    } else {
      const userId = getUserIdFromToken(token);
      permissions = await fetchUserPermissions(userId, token);
    }

    if (permissions) {
      renderButtons(permissions);
    }
  } catch (error) {
    console.error("Error checking permissions:", error);
  }
}

function getUserIdFromToken(token) {
  const payload = JSON.parse(atob(token.split(".")[1]));
  return payload.id;
}

async function fetchUserPermissions(userId, token) {
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

function createButton(type, icon, text, action, btnClass) {
  return `
    <button class="btn ${btnClass} btn-sm" onclick="${action}">
      <i class="${icon}"></i>${text}
    </button>
  `;
}

function createLink(href, icon, text, btnClass) {
  return `
    <a href="${href}" class="btn ${btnClass} btn-sm">
      <i class="${icon}"></i>${text}
    </a>
  `;
}

function renderButtons(permissions) {
  const buttons = [];
  const { delete: deleteText, newNotification: createText } =
    window.notificationResources;

  if (permissions.canDelete) {
    buttons.push(
      createButton(
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
      createLink(
        "/Notifications/Create",
        "fas fa-plus me-1",
        createText,
        "btn-success"
      )
    );
  }

  $(".card-header .d-flex.gap-2").html(buttons.join(""));
}

// Initialize when document is ready
$(document).ready(() => {
  checkPermissions();
});
