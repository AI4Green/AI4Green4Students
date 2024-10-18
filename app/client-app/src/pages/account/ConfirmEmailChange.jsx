import { Container, Text, VStack } from "@chakra-ui/react";
import { BusyPage } from "components/core/Busy";
import { TitledAlert } from "components/TitledAlert";
import { useBackendApi } from "contexts/BackendApi";
import { useUser } from "contexts/User";
import { useQueryStringViewModel } from "helpers/hooks/useQueryStringViewModel";
import { Suspense, useCallback } from "react";
import { useAsync } from "react-use";
import { useTranslation } from "react-i18next";
import { useNavigate } from "react-router-dom";

const ErrorFeedback = ({ tKey }) => {
  const { t } = useTranslation();
  return (
    <Container my={16}>
      <VStack spacing={4}>
        <TitledAlert status="error" title={t("feedback.error_title")}>
          <Text>{t(tKey ?? "confirmEmailChange.feedback.error")}</Text>
        </TitledAlert>
      </VStack>
    </Container>
  );
};

const useConfirmHandler = () => {
  const {
    account: { confirmEmailChange },
  } = useBackendApi();
  return useCallback(
    async ({ userId, token, newEmail }) =>
      await confirmEmailChange(userId, token, newEmail).json(),
    [confirmEmailChange]
  );
};

// this actually does the hard work
// but we use suspense at the page level while its busy
const ChangeEmailConfirmation = () => {
  const { userId, token, newEmail } = useQueryStringViewModel();
  const { signIn } = useUser();
  const navigate = useNavigate();
  const { t } = useTranslation();

  const handleConfirm = useConfirmHandler();

  const { error, value: data } = useAsync(
    async () => await handleConfirm({ userId, token, newEmail }),
    [userId, token, newEmail]
  );

  if (error) {
    let tKey;
    switch (error?.response?.status) {
      case 400:
      case 404:
        tKey = "confirmEmailChange.feedback.invalidLink";
        break;
    }

    console.error(error);
    return <ErrorFeedback tKey={tKey} userId={userId} />;
  }

  if (data) {
    const { user } = data;
    // log in, redirect home and TOAST success
    signIn(user);
    navigate("/", {
      state: {
        toast: {
          title: t("confirmEmailChange.feedback.success"),
          status: "success",
          duration: 2500,
          isClosable: true,
        },
      },
    });
  }

  return null;
};

export const ConfirmEmailChange = () => (
  <Suspense
    fallback={
      <BusyPage
        tKey="confirmEmailChange.feedback.busy"
        containerProps={{ justifyContent: "center" }}
      />
    }
  >
    <ChangeEmailConfirmation />
  </Suspense>
);
