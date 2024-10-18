import {
  HStack,
  Heading,
  VStack,
  Text,
  Icon,
  useDisclosure,
  LinkBox,
  LinkOverlay,
  Box,
} from "@chakra-ui/react";
import { FaCheckCircle, FaExchangeAlt, FaLock } from "react-icons/fa";
import { Link, useNavigate } from "react-router-dom";
import { DefaultContentLayout } from "layouts/DefaultLayout";
import { NotificationBadge } from "components/core/NotificationBadge";
import { Header } from "components/experiment/section/Header";
import { STAGES_PERMISSIONS } from "constants/site-permissions";
import { STAGES } from "constants/stages";
import { useIsInstructor } from "components/experiment/useIsInstructor";
import { useState } from "react";
import { ActionButton } from "components/ActionButton";
import { MoveStageModal } from "components/experiment/modal/MoveStageModal";
import { STATUS_ICON_COMPONENTS } from "constants/experiment-ui";
import { useUser } from "contexts/User";
import { SECTION_TYPES } from "constants/section-types";

const Section = ({ section, path, index, isRecordOwner, isInstructor }) => {
  const { name, approved, comments, stage } = section;
  const ariaQualifier = comments == 1 ? "is " : "are ";
  const ariaPlural = comments == 1 ? "" : "s";
  const ariaApproved = approved
    ? ". Item is approved"
    : ". Incomplete/Unapproved";
  const ariaLinkLabel = `${name} ${ariaApproved} ${
    comments >= 1
      ? `. There ${ariaQualifier} ${comments} comment${ariaPlural} on this item`
      : ""
  }`;

  const statusIndicator = {
    icon: approved ? FaCheckCircle : STATUS_ICON_COMPONENTS[stage]?.icon,
    color: approved
      ? "green.500"
      : STATUS_ICON_COMPONENTS[stage]?.color || "gray",
    ariaLabel: approved ? "Approved" : stage,
  };

  return (
    <LinkBox w="full" borderBottomWidth={1} p={2} borderRadius={5}>
      <HStack
        borderRadius={8}
        gap={2}
        _hover={{
          bg: "gray.50",
        }}
        p={2}
      >
        <Text>{index + 1}</Text>

        <LinkOverlay as={Link} to={path} aria-label={ariaLinkLabel}>
          <Heading as="h4" size="sm">
            {name}
          </Heading>
        </LinkOverlay>

        <Box display="flex" justifyContent="flex-end" flex={1}>
          {(isRecordOwner || isInstructor) && comments >= 1 && !approved ? (
            <VStack align="flex-end">
              <NotificationBadge
                count={comments > 9 ? "9+" : comments}
                to={path}
              />
              <Text fontSize="xs">Unread comments</Text>
            </VStack>
          ) : (
            statusIndicator?.icon && (
              <VStack align="flex-end">
                <Icon
                  as={statusIndicator.icon}
                  color={statusIndicator.color}
                  aria-label={statusIndicator.ariaLabel}
                />
                <Text fontSize="xs">{statusIndicator.ariaLabel}</Text>
              </VStack>
            )
          )}
        </Box>
      </HStack>
    </LinkBox>
  );
};

export const Overview = ({
  sections,
  headerItems,
  InstructorAction,
  breadcrumbs,
}) => {
  const isInstructor = useIsInstructor();
  const { user } = useUser();
  const { ownerId } = headerItems;

  return (
    <DefaultContentLayout>
      {breadcrumbs}
      <Header
        {...headerItems}
        actionSection={isInstructor && InstructorAction}
      />
      <VStack
        align="stretch"
        minW={{ base: "full", md: "95%", lg: "80%", xl: "70%" }}
        spacing={8}
      >
        {sections && sections.length >= 1 ? (
          sections
            .sort((a, b) => a.sortOrder - b.sortOrder)
            .map((section, index) => (
              <Section
                key={section.id}
                section={section}
                path={section.path}
                index={index}
                isRecordOwner={ownerId === user.userId}
                isInstructor={isInstructor}
              />
            ))
        ) : (
          <Text fontSize="lg">No sections available</Text>
        )}
      </VStack>
    </DefaultContentLayout>
  );
};

export const InstructorAction = ({
  record,
  isEverySectionApproved,
  sectionType,
}) => {
  const [modalActionProps, setModalActionProps] = useState({
    modalTitle: "Confirmation",
    fixedNextStage: null,
    successMessage: "Success",
  });

  const {
    isOpen: isOpenAdvanceStage,
    onOpen: onOpenAdvanceStage,
    onClose: onCloseAdvanceStage,
  } = useDisclosure();
  const navigate = useNavigate();

  const actions = createInstructorActions({
    record,
    sectionType,
    isEverySectionApproved,
    onOpenAdvanceStage,
    setModalActionProps,
    navigate,
  });

  return (
    <>
      <ActionButton
        actions={actions}
        size="sm"
        variant="outline"
        colorScheme={
          STATUS_ICON_COMPONENTS[record.stage]?.color.split(".")[0] || // can extract base color. e.g. "green.500" -> "green"
          "gray"
        }
        label={record.stage}
        LeftIcon={STATUS_ICON_COMPONENTS[record.stage]?.icon}
      />
      {isOpenAdvanceStage && (
        <MoveStageModal
          isModalOpen={isOpenAdvanceStage}
          onModalClose={onCloseAdvanceStage}
          record={record}
          sectionType={sectionType}
          mutate={record.mutate}
          {...modalActionProps}
        />
      )}
    </>
  );
};

const createInstructorActions = ({
  record,
  sectionType,
  isEverySectionApproved,
  onOpenAdvanceStage,
  setModalActionProps,
}) => {
  return {
    requestChanges: {
      isEligible: () =>
        record.permissions.includes(STAGES_PERMISSIONS.InstructorCanComment),
      icon: <FaExchangeAlt />,
      label: "Request Changes",
      onClick: () => {
        setModalActionProps({
          modalTitle: "Request Changes",
          modalMessage:
            "Do you wish to proceed with requesting changes for the following?",
          fixedNextStage: STAGES.AwaitingChanges,
          successMessage: "Request changes succeeded",
          failMessage: "Request changes failed",
        });
        onOpenAdvanceStage();
      },
    },
    cancelRequestChanges: {
      isEligible: () => record.stage === STAGES.AwaitingChanges,
      icon: <FaExchangeAlt />,
      label: "Cancel Request Changes",
      onClick: () => {
        setModalActionProps({
          modalTitle: "Cancel Request Changes",
          modalMessage:
            "Do you wish to proceed with cancelling the request changes for the following?",
          fixedNextStage: STAGES.InReview,
          successMessage: "Request changes cancellation succeeded",
          failMessage: "Request changes cancellation failed",
        });
        onOpenAdvanceStage();
      },
    },
    markAsApproved: {
      isEligible: () =>
        record.permissions.includes(STAGES_PERMISSIONS.InstructorCanComment) &&
        isEverySectionApproved,
      icon: <FaCheckCircle />,
      label: "Mark as approved",
      onClick: () => {
        setModalActionProps({
          modalTitle: "Mark as approved",
          modalMessage:
            "Do you wish to proceed with marking the following as approved?",
          fixedNextStage: STAGES.Approved,
          successMessage: "Mark as approved succeeded",
          failMessage: "Mark as approved failed",
        });
        onOpenAdvanceStage();
      },
    },
    cancelApproval: {
      isEligible: () => record.stage === STAGES.Approved,
      icon: <FaCheckCircle />,
      label: "Cancel Approval",
      onClick: () => {
        setModalActionProps({
          modalTitle: "Cancel Approval",
          modalMessage:
            "Do you wish to proceed with cancelling the approval for the following?",
          fixedNextStage: STAGES.InReview,
          successMessage: "Approval cancellation succeeded",
          failMessage: "Approval cancellation  failed",
        });
        onOpenAdvanceStage();
      },
    },
    cancelNotesLock: {
      isEligible: () =>
        record.stage === STAGES.Locked && sectionType === SECTION_TYPES.Note,
      icon: <FaLock />,
      label: "Cancel Note Lock",
      onClick: () => {
        setModalActionProps({
          modalTitle: "Cancel Note Lock",
          modalMessage:
            "Do you wish to proceed with cancelling the note lock for the following?",
          fixedNextStage: STAGES.Draft,
          successMessage: "Note lock cancellation succeeded",
          failMessage: "Note lock cancellation  failed",
        });
        onOpenAdvanceStage();
      },
    },
  };
};
