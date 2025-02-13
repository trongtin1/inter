const moduleService = {
  renderModulePermissions(modulePermissions, container) {
    container.empty();

    modulePermissions.forEach((permissions, moduleId) => {
      container.append(`
                <div class="module-permission mb-2 p-3 border rounded" data-module-name="${permissions.name.toLowerCase()}">
                    <h6 class="mb-3">${permissions.name}</h6>
                    <div class="row g-2">
                        ${this.renderPermissionCheckbox(
                          moduleId,
                          "create",
                          permissions.canCreate
                        )}
                        ${this.renderPermissionCheckbox(
                          moduleId,
                          "read",
                          permissions.canRead
                        )}
                        ${this.renderPermissionCheckbox(
                          moduleId,
                          "update",
                          permissions.canUpdate
                        )}
                        ${this.renderPermissionCheckbox(
                          moduleId,
                          "delete",
                          permissions.canDelete
                        )}
                    </div>
                </div>
            `);
    });
  },

  renderPermissionCheckbox(moduleId, type, isEnabled) {
    return `
            <div class="col-md-3">
                <div class="form-check">
                    <input type="checkbox" class="form-check-input" 
                           id="${type}_${moduleId}" 
                           ${isEnabled ? "checked" : ""} 
                           ${!isEnabled ? "disabled" : ""}>
                    <label class="form-check-label ${
                      !isEnabled ? "text-muted" : ""
                    }" 
                           for="${type}_${moduleId}">${
      type.charAt(0).toUpperCase() + type.slice(1)
    }</label>
                </div>
            </div>
        `;
  },

  getModulePermissions() {
    const permissions = [];
    $("#modulePermissions .module-permission").each(function () {
      const moduleId = $(this).find("input").first().attr("id").split("_")[1];
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
};
