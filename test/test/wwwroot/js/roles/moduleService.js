const moduleService = {
  async getModules() {
    const response = await axios.get("/api/Modules/getList");
    return response.data.data;
  },

  renderModulePermissions(modules, container) {
    container.empty();

    modules.forEach((module) => {
      container.append(`
                <div class="module-permission mb-4 p-3 border rounded" data-module-id="${
                  module.id
                }" data-module-name="${module.name.toLowerCase()}">
                    <h6 class="mb-3">${module.name}</h6>
                    <div class="row g-2">
                        ${this.renderPermissionCheckbox(module.id, "create")}
                        ${this.renderPermissionCheckbox(module.id, "read")}
                        ${this.renderPermissionCheckbox(module.id, "update")}
                        ${this.renderPermissionCheckbox(module.id, "delete")}
                    </div>
                </div>
            `);
    });

    // Initialize search functionality after rendering
    this.setupModuleSearch();
  },

  renderPermissionCheckbox(moduleId, type) {
    return `
            <div class="col-md-3">
                <div class="form-check">
                    <input type="checkbox" class="form-check-input" 
                           id="${type}_${moduleId}" 
                           name="permissions[${moduleId}][${type}]">
                    <label class="form-check-label" 
                           for="${type}_${moduleId}">
                        ${type.charAt(0).toUpperCase() + type.slice(1)}
                    </label>
                </div>
            </div>
        `;
  },

  getModulePermissions() {
    const permissions = [];
    $(".module-permission").each(function () {
      const moduleId = $(this).data("module-id");
      permissions.push({
        moduleId: moduleId,
        canCreate: $(`#create_${moduleId}`).is(":checked"),
        canRead: $(`#read_${moduleId}`).is(":checked"),
        canUpdate: $(`#update_${moduleId}`).is(":checked"),
        canDelete: $(`#delete_${moduleId}`).is(":checked"),
      });
    });
    return permissions;
  },

  setupModuleSearch() {
    $("#moduleSearch").on("input", function () {
      const searchTerm = $(this).val().toLowerCase();
      $(".module-permission").each(function () {
        const moduleName = $(this).find("h6").text().toLowerCase();
        $(this).toggle(moduleName.includes(searchTerm));
      });
    });
  },
};
