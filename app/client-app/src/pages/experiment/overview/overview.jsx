import {
  Box,
  Heading,
  HStack,
  Icon,
  LinkBox,
  LinkOverlay,
  Text,
  VStack,
} from "@chakra-ui/react";
import { Breadcrumbs } from "components/core/breadcrumbs";
import { NotificationBadge } from "components/core/notification-badge";
import { SectionHeader } from "components/section-header/header";
import { STATUS_ICON_COMPONENTS } from "constants";
import { DefaultContentLayout } from "layouts/default";
import { FaCheckCircle } from "react-icons/fa";
import { Link } from "react-router-dom";

export const Overview = ({
  item,
  sections,
  headerItems,
  breadcrumbs,
  isInstructor,
}) => {
  return (
    <DefaultContentLayout>
      <Breadcrumbs items={breadcrumbs} />
      <SectionHeader {...headerItems} />
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
                index={index}
                canViewComments={item.isOwner || isInstructor}
                stage={item.stage.name}
              />
            ))
        ) : (
          <Text fontSize="lg">No sections available</Text>
        )}
      </VStack>
    </DefaultContentLayout>
  );
};

const Section = ({ section, index, canViewComments, stage }) => {
  const { name } = section;
  const { approved, comments } = section?.feedback || {};
  const ariaQualifier = comments?.unread == 1 ? "is " : "are ";
  const ariaPlural = comments?.unread == 1 ? "" : "s";
  const ariaApproved = approved
    ? ". Item is approved"
    : ". Incomplete/Unapproved";
  const ariaLinkLabel = `${name} ${ariaApproved} ${
    comments?.unread >= 1
      ? `. There ${ariaQualifier} ${comments.unread} comment${ariaPlural} on this item`
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
    <LinkBox w="full" borderBottomWidth={0.8} p={2}>
      <HStack
        _hover={{
          bg: "gray.50",
          transition: "all 0.4s ease-in-out",
        }}
        p={4}
        borderRadius={4}
      >
        <Text>{index + 1}</Text>

        <LinkOverlay as={Link} to={section.path} aria-label={ariaLinkLabel}>
          <Heading as="h4" size="sm">
            {name}
          </Heading>
        </LinkOverlay>

        <Box display="flex" justifyContent="flex-end" flex={1}>
          {canViewComments && comments?.unread >= 1 && !approved ? (
            <VStack align="flex-end">
              <NotificationBadge
                count={comments.unread > 9 ? "9+" : comments.unread}
                to={section.path}
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
