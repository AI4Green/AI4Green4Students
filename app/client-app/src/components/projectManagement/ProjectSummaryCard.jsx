import { Link } from "react-router-dom";
import {
  VStack,
  Box,
  Badge,
  LinkOverlay,
  Heading,
  LinkBox,
} from "@chakra-ui/react";

export const ProjectSummaryCard = ({
  href = "#",
  title,
  subtitle,
  isProject,
}) => {
  return (
    <LinkBox
      w="230px"
      minH="225px"
      borderWidth={1}
      p={4}
      m={2}
      bgGradient="radial(circle 600px at top left, green.50,
      blue.100)"
      display="flex"
      flexDirection="column"
    >
      <Box>
        <Badge
          borderRadius={7}
          py={0.5}
          px={3}
          colorScheme={isProject ? "green" : "cyan"}
        >
          {isProject ? "Project" : "Project Group"}
        </Badge>
      </Box>
      <VStack align="flex-start" h="full" justify="center" spacing={1}>
        <LinkOverlay as={Link} to={href}>
          <Heading size="md" fontWeight="bold">
            {title}
          </Heading>
        </LinkOverlay>
        <Heading size="xs" fontWeight="normal">
          {subtitle}
        </Heading>
      </VStack>
    </LinkBox>
  );
};
