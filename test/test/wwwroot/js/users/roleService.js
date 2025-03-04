const roleService = {
  async getRoles() {
    const response = await axios.get("/api/Roles/getList");
    return response.data.data;
  },

  async getRoleModules(roleId) {
    const response = await axios.get(`/api/RoleModules/${roleId}`);
    return response.data.data.modules;
  },

  renderRoles(roles, container) {
    container.empty();
    roles.forEach((role) => {
      container.append(`
                <div class="form-check mb-2">
                    <input class="form-check-input role-checkbox" type="checkbox" 
                           value="${role.id}" id="role${role.id}" 
                           data-role-name="${role.name}">
                    <label class="form-check-label" for="role${role.id}">
                        ${role.name}
                    </label>
                </div>
            `);
    });
  },
};
