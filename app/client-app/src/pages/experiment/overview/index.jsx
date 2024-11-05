import {
  HStack,
  Heading,
  VStack,
  Text,
  Icon,
  LinkBox,
  LinkOverlay,
  Box,
} from "@chakra-ui/react";
import { FaCheckCircle } from "react-icons/fa";
import { Link } from "react-router-dom";
import { DefaultContentLayout } from "layouts/DefaultLayout";
import { NotificationBadge } from "components/core/NotificationBadge";
import { Header } from "components/experiment/section/Header";
import { useIsInstructor } from "components/experiment/useIsInstructor";
import { STATUS_ICON_COMPONENTS } from "constants/experiment-ui";
import { useUser } from "contexts/User";

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
  StudentAction,
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
        actionSection={isInstructor ? InstructorAction : StudentAction}
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
