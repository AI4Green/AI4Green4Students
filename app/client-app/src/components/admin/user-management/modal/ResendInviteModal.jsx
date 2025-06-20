import { useToast } from "@chakra-ui/react";
import { useBackendApi } from "contexts";
import { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";
import { errorMessage } from "./error-message";
import { GLOBAL_PARAMETERS } from "constants";

export const ResendInviteModal = ({ user, onModalClose }) => {
  const [feedback, setFeedback] = useState();
  const { t } = useTranslation();
  const toast = useToast();

  const { users } = useBackendApi();

  useEffect(() => {
    const handleSubmit = async () => {
      try {
        const response = await users.resendInvite(user.id);

        if (response && response.status === 204) {
          toast({
            title: `User invited re-sent to ${user.email}`,
            status: "success",
            duration: GLOBAL_PARAMETERS.ToastDuration,
            isClosable: true,
            position: "top",
          });
          onModalClose();
        }
      } catch (e) {
        const { messageError } = await errorMessage(e, t);
        setFeedback({
          status: "error",
          message: messageError,
        });
      }
    };
    handleSubmit();
  }, []);

  useEffect(() => {
    if (!feedback) return;
    toast({
      title: feedback.message,
      status: feedback.status,
      duration: GLOBAL_PARAMETERS.ToastDuration,
      isClosable: true,
      position: "top",
    });
  }, [feedback]);

  return null;
};
