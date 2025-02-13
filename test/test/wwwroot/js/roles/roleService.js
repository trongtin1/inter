const roleService = {
  async createRole(roleData) {
    try {
      const response = await axios.post("/api/roles", roleData);
      if (!response.data.success) {
        throw new Error(response.data.message || "Failed to create role");
      }
      return response.data.data;
    } catch (error) {
      console.error("Error creating role:", error);
      throw error;
    }
  },

  async assignRoleModules(roleId, modulePermissions) {
    try {
      roleId = parseInt(roleId);

      const promises = modulePermissions.map((permission) => {
        const data = {
          roleId: roleId,
          moduleId: parseInt(permission.moduleId),
          canCreate: permission.canCreate,
          canRead: permission.canRead,
          canUpdate: permission.canUpdate,
          canDelete: permission.canDelete,
        };
        return axios.post("/api/RoleModules", data);
      });

      const results = await Promise.all(promises);
      return results;
    } catch (error) {
      console.error("Error assigning role modules:", error);
      throw error;
    }
  },
};
