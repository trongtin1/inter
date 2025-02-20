$(document).ready(function () {
  initializeForm();
  setupEventHandlers();
});

function initializeForm() {
  loadModules();
}

async function loadModules() {
  try {
    const modules = await moduleService.getModules();
    moduleService.renderModulePermissions(modules, $("#modulePermissions"));
  } catch (error) {
    console.error("Error loading modules:", error);
    alert("Failed to load modules");
  }
}

function setupEventHandlers() {
  $("#createRoleForm").on("submit", async function (e) {
    e.preventDefault();
    await createRoleWithModules();
  });
}

async function createRoleWithModules() {
  try {
    // 1. Create role first
    const roleData = {
      name: $("#name").val(),
    };

    const role = await roleService.createRole(roleData);

    if (!role || !role.id) {
      throw new Error("Failed to get role ID after creation");
    }

    // 2. Get module permissions
    const modulePermissions = moduleService.getModulePermissions();

    if (modulePermissions.length === 0) {
      alert("Please select at least one module permission");
      return;
    }

    // 3. Assign module permissions to role
    await roleService.assignRoleModules(role.id, modulePermissions);

    alert("Role created successfully with module permissions");
    window.location.href = "/Roles";
  } catch (error) {
    console.error("Error in role creation process:", error);
    if (error.response) {
      alert(
        error.response.data.message ||
          "Failed to complete role creation process"
      );
    } else {
      alert(error.message || "Failed to complete role creation process");
    }
  }
}
