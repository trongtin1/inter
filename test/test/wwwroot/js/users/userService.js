const userService = {
  async createUser(userData) {
    const response = await axios.post("/api/users", userData);
    if (!response.data.success) {
      throw new Error(response.data.message || "Failed to create user");
    }
    return response.data.data;
  },

  async assignUserRoles(userRolesData) {
    const response = await axios.post("/api/UserRoles", userRolesData);
    if (!response.data.success) {
      throw new Error(response.data.message || "Failed to assign roles");
    }
    return response.data;
  },

  async assignUserModulePermissions(moduleData) {
    const response = await axios.post("/api/UserModules", moduleData);
    if (!response.data.success) {
      throw new Error(
        response.data.message ||
          `Failed to assign module permissions for module ${moduleData.ModuleId}`
      );
    }
    return response.data;
  },
};
