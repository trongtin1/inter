const permissionHandler = {
  async initializeModulePermissions() {
    const token = sessionStorage.getItem("token");
    if (!token) return null;

    try {
      const payload = JSON.parse(atob(token.split(".")[1]));
      if (!payload || !payload.id) return null;

      const response = await axios.get(
        `http://localhost:5001/api/UserModules/${payload.id}`,
        {
          headers: { Authorization: `Bearer ${token}` },
        }
      );

      if (response?.data?.success) {
        return response.data.data.modules;
      }
      return null;
    } catch (error) {
      console.error("Error fetching user permissions:", error);
      return null;
    }
  },

  updateMenuVisibility(modules) {
    if (!modules) return;

    // Kiểm tra quyền cho từng module
    modules.forEach((module) => {
      switch (module.moduleName) {
        case "Mails":
          $('.nav-item a[href="/Mails"]').closest("li").toggle(module.canRead);
          break;
        case "Statistics":
          $('.nav-item a[href="/Statistics"]')
            .closest("li")
            .toggle(module.canRead);
          break;
        case "Admin":
          $('.nav-item a[href="/Admin"]').closest("li").toggle(module.canRead);
          break;
        // Thêm các module khác tại đây
      }
    });
  },
};
