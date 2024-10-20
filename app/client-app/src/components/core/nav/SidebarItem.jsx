import {
  Box,
  Text,
  Icon,
  LinkBox,
  LinkOverlay,
  HStack,
} from "@chakra-ui/react";
import { Link } from "react-router-dom";

export const SidebarButton = ({ item, onClose }) => {
  if (!item.path) {
    return <SidebarTitle item={item} />;
  }

  return <SidebarLinkButton item={item} onClose={onClose} />;
};

const SidebarLinkButton = ({ item, onClose }) => (
  <LinkBox
    w="full"
    py={{ md: 1, lg: 2 }}
    px={{ md: 0, lg: 4 }}
    _hover={{
      bg: "gray.100",
      borderRadius: 7,
    }}
  >
    <LinkOverlay as={Link} to={item.path} onClick={onClose}>
      <HStack>
        <Icon as={item.icon} boxSize={5} marginRight={2} />
        <Text
          fontSize={{ md: "sm", lg: "md" }}
          fontWeight="semibold"
          letterSpacing="tight"
          py={1}
        >
          {item.name}
        </Text>
      </HStack>
    </LinkOverlay>
  </LinkBox>
);

const SidebarTitle = ({ item }) => {
  return (
    <Box display="flex" alignItems="center">
      <Icon as={item.icon} boxSize={5} marginRight={2} />
      <Text fontSize="lg" fontWeight="semibold" letterSpacing="tight" py={1}>
        {item.name}
      </Text>
    </Box>
  );
};

export default SidebarButton;
