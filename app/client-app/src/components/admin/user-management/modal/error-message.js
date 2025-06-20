export const errorMessage = async (e, t) => {
  let result;
  if (e?.response) {
    const textResponse = await e.response.text();
    try {
      result = JSON.parse(textResponse);
    } catch {
      result = { message: textResponse };
    }
  }

  let messageError = t("feedback.error_title");
  switch (e?.response?.status) {
    case 400:
      {
        let m = t("feedback.error_400");
        if (result?.isExistingUser) m = "User already exists";
        else if (result?.isExistingEmail) m = "Email already exists";
        else if (result?.isNotAllowlisted)
          m = "User not eligible for registration";
        else if (result?.IsRolesNotValidOrSelected)
          m = "Roles are not valid or selected";
        messageError = result?.message ?? m;
      }
      break;
    case 401:
      messageError = result?.message ?? t("feeback.error_401");
      break;
    case 403:
      messageError = result?.message ?? t("feedback.error_403");
      break;
    case 404:
      messageError = result?.message ?? t("feedback.error_404");
      break;
    case 500:
      messageError = result?.message ?? t("feedback.error_title");
      break;
  }
  return { messageError };
};
