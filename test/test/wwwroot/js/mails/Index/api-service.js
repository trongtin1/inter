const mailApiService = {
  baseUrl: "http://localhost:5001/api",

  // Check if token is expired
  isTokenExpired(token) {
    try {
      const base64Url = token.split(".")[1];
      const base64 = base64Url.replace(/-/g, "+").replace(/_/g, "/");
      const payload = JSON.parse(window.atob(base64));

      // exp is in seconds, Date.now() is in milliseconds
      return payload.exp * 1000 < Date.now();
    } catch {
      return true; // If there's any error parsing, consider token as expired
    }
  },

  // Get auth token
  getToken() {
    const token = localStorage.getItem("token");
    if (!token || this.isTokenExpired(token)) {
      sessionStorage.removeItem("token"); // Clear invalid token
      window.location.href = "/Account/Login";
      return null;
    }
    return token;
  },

  // API calls with caching
  async getMails(params) {
    const token = this.getToken();
    if (!token) return null;

    try {
      const response = await axios.get(`${this.baseUrl}/Mails?${params}`, {
        headers: { Authorization: `Bearer ${token}` },
      });
      return response.data;
    } catch (error) {
      this.handleError(error);
      return null;
    }
  },

  async getFilterOptions() {
    const token = this.getToken();
    if (!token) return null;

    try {
      const response = await axios.get(`${this.baseUrl}/Mails/filter-options`, {
        headers: { Authorization: `Bearer ${token}` },
      });
      return response.data;
    } catch (error) {
      this.handleError(error);
      return null;
    }
  },

  async deleteMultipleMails(ids) {
    const token = this.getToken();
    if (!token) return null;

    try {
      const response = await axios.post(
        `${this.baseUrl}/Mails/deleteMultiple`,
        ids,
        {
          headers: { Authorization: `Bearer ${token}` },
        }
      );
      return response.data;
    } catch (error) {
      this.handleError(error);
      return null;
    }
  },

  handleError(error) {
    console.error("API Error:", error);
    if (error.response?.status === 401) {
      window.location.href = "/Account/Login";
    }
    throw error;
  },
};
