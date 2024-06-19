import { useToast } from "@chakra-ui/react";
import { Modal } from "components/Modal";
import { useBackendApi } from "contexts/BackendApi";
import { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";
import { DisplayLink } from "../DisplayLink";
import { errorMessage } from "../error-message";
import { GLOBAL_PARAMETERS } from "constants/global-parameters";

export const ResendInviteModal = ({ user, isModalOpen, onModalClose }) => {
  const activationLinkKey = "activationLink";
  const [generatedLink, setGeneratedLink] = useState();
  const [isLoading, setIsLoading] = useState();
  const [feedback, setFeedback] = useState();
  const { t } = useTranslation();
  const toast = useToast();

  const { account } = useBackendApi();

  useEffect(() => {
    const handleSubmit = async () => {
      try {
        setIsLoading(true);
        const response = await account.resendInvite(user.id);
        setIsLoading(false);

        if (response && response.status === 200) {
          const parsed = await response?.json();
          setGeneratedLink(parsed[activationLinkKey] ?? "");
          toast({
            title: "User invite re-send link generated",
            status: "success",
            duration: GLOBAL_PARAMETERS.ToastDuration,
            isClosable: true,
            position: "top",
          });
          return;
        }

        if (response && response.status === 204) {
          toast({
            title: `User invited re-sent to ${user.email}`,
            status: "success",
            duration: GLOBAL_PARAMETERS.ToastDuration,
            isClosable: true,
            position: "top",
          });
        }
      } catch (e) {
        const { messageError } = await errorMessage(e, t);
        setFeedback({
          status: "error",
          message: messageError,
        });
      }
      onModalClose();
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

  if (generatedLink) {
    return (
      <Modal
        body={<DisplayLink displayLink={generatedLink} linkType="activation" />}
        title="Resend invite"
        actionBtnCaption="Ok"
        onAction={onModalClose}
        actionBtnColorScheme="green"
        isLoading={isLoading}
        isOpen={isModalOpen}
        onClose={onModalClose}
        cancelBtnEnable={!generatedLink}
      />
    );
  }
  return null;
};
