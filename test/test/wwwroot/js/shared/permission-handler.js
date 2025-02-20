const permissionHandler = {
  async initializeModulePermissions() {
    // Kiểm tra cache trước
    const cachedModules = sessionStorage.getItem("userModules");
    if (cachedModules) {
      return JSON.parse(cachedModules);
    }

    const token = localStorage.getItem("token");
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
        // Cache kết quả
        sessionStorage.setItem(
          "userModules",
          JSON.stringify(response.data.data.modules)
        );
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

    const navBar = $(".navbar-nav.flex-grow-1");
    let dynamicMenuItems = "";

    // Thêm các menu dựa trên quyền
    modules.forEach((module) => {
      if (module.canRead) {
        const translatedName =
          window.menuResources[module.moduleName.toLowerCase()] ||
          module.moduleName;
        dynamicMenuItems += `
          <li class="nav-item">
            <a class="nav-link text-dark" href="/${module.moduleName}">${translatedName}</a>
          </li>
        `;
      }
    });

    // Thêm menu động vào sau các menu tĩnh có sẵn
    navBar.append(dynamicMenuItems);
  },
};
