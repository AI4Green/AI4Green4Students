import { useToast } from "@chakra-ui/react";
import { Modal, useModalState } from "components/core/modal";
import { GLOBAL_PARAMETERS, TITLE_ICON_COMPONENTS } from "constants";
import { useBackendApi } from "contexts";
import { ConfirmationModal as DeleteConfirmationModal } from "layouts/default";
import { useEffect } from "react";
import { useTranslation } from "react-i18next";
import { useLocation, useNavigate, useSearchParams } from "react-router-dom";

export const DeleteModal = ({ list, mutate }) => {
  const [searchParams] = useSearchParams();
  const id = searchParams.get("id");

  const navigate = useNavigate();
  const location = useLocation();

  const { registrationRules: action } = useBackendApi();

  const { t } = useTranslation();
  const toast = useToast();

  const {
    isModalOpen,
    setIsModalOpen,
    isLoading,
    setIsLoading,
    feedback,
    setFeedback,
    handleReset,
  } = useModalState(location, navigate);

  const registrationRule = list?.find((x) => x.id === Number(id));

  useEffect(() => {
    setIsModalOpen(true);
  }, [id, setIsModalOpen]);

  const handleDelete = async () => {
    try {
      setIsLoading(true);
      const response = await action.delete(id);
      setIsLoading(false);

      if (response && (response.status === 204 || response.status === 200)) {
        toast({
          title: "Registration rule deleted",
          status: "success",
          duration: GLOBAL_PARAMETERS.ToastDuration,
          isClosable: true,
          position: "top",
        });
        handleReset();
        await mutate();
      }
    } catch (e) {
      switch (e.response.status) {
        case 404: {
          setFeedback({
            status: "error",
            message: t("feedback.error_404"),
          });
          break;
        }
        default: {
          setFeedback({
            status: "error",
            message: t("feedback.error_title"),
          });
        }
      }
    }
  };

  return (
    <Modal
      body={
        <DeleteConfirmationModal
          feedback={feedback}
          content={{
            tags: [
              {
                label: "Registration Rule",
                leftIcon: TITLE_ICON_COMPONENTS.RegistrationRule,
              },
            ],
            value: registrationRule.value,
            description:
              "Are you sure you want to delete this registration rule?",
          }}
        />
      }
      title="Delete Confirmation"
      actionBtnCaption="Delete"
      actionBtnColorScheme="red"
      onAction={handleDelete}
      isLoading={isLoading}
      isOpen={isModalOpen}
      onClose={handleReset}
    />
  );
};
