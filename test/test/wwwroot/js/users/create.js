$(document).ready(function () {
  initializeForm();
  setupEventHandlers();
});

function initializeForm() {
  loadRolesAndModules();
}

function setupEventHandlers() {
  // Password toggle
  $(".toggle-password").click(handlePasswordToggle);

  // Search functionalities
  $("#roleSearch").on("input", handleRoleSearch);
  $("#moduleSearch").on("input", handleModuleSearch);

  // Form submission
  $("#createUserForm").on("submit", async function (e) {
    e.preventDefault();
    await createUserWithRolesAndModules();
  });

  // Password confirmation
  $("#confirmPassword").on("input", handlePasswordConfirmation);
}

function handlePasswordToggle() {
  const targetId = $(this).data("target");
  const input = $(`#${targetId}`);
  const icon = $(this).find("i");

  if (input.attr("type") === "password") {
    input.attr("type", "text");
    icon.removeClass("bi-eye").addClass("bi-eye-slash");
  } else {
    input.attr("type", "password");
    icon.removeClass("bi-eye-slash").addClass("bi-eye");
  }
}

function handleRoleSearch() {
  const searchTerm = $(this).val().toLowerCase();
  $(".form-check").each(function () {
    const roleText = $(this).find("label").text().toLowerCase();
    $(this).toggle(roleText.includes(searchTerm));
  });
}

function handleModuleSearch() {
  const searchTerm = $(this).val().toLowerCase();
  $(".module-permission").each(function () {
    const moduleText = $(this).find("h6").text().toLowerCase();
    $(this).toggle(moduleText.includes(searchTerm));
  });
}

function handlePasswordConfirmation() {
  const password = $("#password").val();
  const confirmPassword = $(this).val();
  $("#passwordMismatch").toggle(password !== confirmPassword);
}

async function loadRolesAndModules() {
  try {
    const roles = await roleService.getRoles();
    roleService.renderRoles(roles, $("#rolesCheckboxList"));

    // Add role checkbox change handler
    $(".role-checkbox").on("change", updateModulePermissions);
  } catch (error) {
    console.error("Error loading roles and modules:", error);
    alert("Failed to load roles and modules");
  }
}

async function updateModulePermissions() {
  const selectedRoles = $(".role-checkbox:checked")
    .map(function () {
      return $(this).val();
    })
    .get();

  if (selectedRoles.length === 0) {
    $("#modulePermissions").empty();
    return;
  }

  try {
    const modulePermissions = new Map();

    for (const roleId of selectedRoles) {
      const modules = await roleService.getRoleModules(roleId);

      modules.forEach((module) => {
        if (!modulePermissions.has(module.moduleId)) {
          modulePermissions.set(module.moduleId, {
            name: module.moduleName,
            canCreate: module.canCreate,
            canRead: module.canRead,
            canUpdate: module.canUpdate,
            canDelete: module.canDelete,
          });
        }
      });
    }

    moduleService.renderModulePermissions(
      modulePermissions,
      $("#modulePermissions")
    );
  } catch (error) {
    console.error("Error updating module permissions:", error);
  }
}

async function createUserWithRolesAndModules() {
  const password = $("#password").val();
  const confirmPassword = $("#confirmPassword").val();

  if (password !== confirmPassword) {
    $("#passwordMismatch").show();
    return;
  }

  try {
    // Create user
    const userData = {
      username: $("#username").val(),
      email: $("#email").val(),
      password: password,
    };
    const user = await userService.createUser(userData);

    // Assign roles
    const selectedRoleIds = $(".role-checkbox:checked")
      .map(function () {
        return parseInt($(this).val());
      })
      .get();

    if (selectedRoleIds.length > 0) {
      await userService.assignUserRoles({
        UserId: parseInt(user.id),
        RoleIds: selectedRoleIds,
      });
    }

    // Assign module permissions
    const modulePermissions = moduleService.getModulePermissions();
    for (const permission of modulePermissions) {
      await userService.assignUserModulePermissions({
        UserId: parseInt(user.id),
        ModuleId: parseInt(permission.moduleId),
        CanCreate: permission.canCreate,
        CanRead: permission.canRead,
        CanUpdate: permission.canUpdate,
        CanDelete: permission.canDelete,
      });
    }

    alert("User created successfully with roles and permissions");
    window.location.href = "/Users";
  } catch (error) {
    console.error("Error in user creation process:", error);
    if (error.response) {
      console.error("Error response:", error.response.data);
      alert(
        error.response.data.message ||
          "Failed to complete user creation process"
      );
    } else {
      alert(error.message || "Failed to complete user creation process");
    }
  }
}
