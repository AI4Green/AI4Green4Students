import {
  Alert,
  AlertIcon,
  HStack,
  Icon,
  Text,
  useToast,
  VStack,
} from "@chakra-ui/react";
import { useRegistrationRulesList } from "api";
import { Badge } from "components/core/Badge";
import { Modal, useModalState } from "components/core/modal";
import { GLOBAL_PARAMETERS } from "constants";
import { useBackendApi } from "contexts";
import { useEffect } from "react";
import { useTranslation } from "react-i18next";
import { FaExclamationTriangle } from "react-icons/fa";
import { useLocation, useNavigate, useSearchParams } from "react-router-dom";

export const DeleteModal = () => {
  const [searchParams] = useSearchParams();
  const id = searchParams.get("id");

  const navigate = useNavigate();
  const location = useLocation();

  const { registrationRules: action } = useBackendApi();
  const { data: list, mutate } = useRegistrationRulesList();

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

  const modalBody = (
    <HStack>
      <Icon as={FaExclamationTriangle} color="red.500" fontSize="5xl" />

      <VStack align="flex-end" flex={1}>
        {feedback && (
          <Alert status={feedback.status}>
            <AlertIcon />
            {feedback.message}
          </Alert>
        )}
        <Text>Are you sure you want to delete this registration rule?</Text>

        <VStack
          align="flex-start"
          borderWidth={1}
          borderRadius={7}
          p={2}
          w="full"
          spacing={2}
        >
          <Text fontWeight="semibold" fontSize="sm">
            {registrationRule.value}
          </Text>
          <Badge label="Registration Rule" fontSize="xxs" />
        </VStack>
      </VStack>
    </HStack>
  );

  return (
    <Modal
      body={modalBody}
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
