export const errorMessage = async (e, t) => {
  let messageError = t("feedback.error_title");
  switch (e?.response?.status) {
    case 400:
      {
        const result = await e.response.json();
        let m = t("feedback.error_400");
        if (result.isExistingUser) m = "User already exists";
        else if (result.isExistingEmail) m = "Email already exists";
        else if (result.isNotAllowlisted)
          m = "User not eligible for registration";
        else if (result.IsRolesNotValidOrSelected)
          m = "Roles are not valid or selected";
        messageError = m;
      }
      break;
    case 401:
      messageError = t("feeback.error_401");
      break;
    case 403:
      messageError = t("feedback.error_403");
      break;
    case 404:
      messageError = t("feedback.error_404");
      break;
    case 500:
      messageError = t("feedback.error_title");
      break;
  }
  return { messageError };
};
