import { IconButton, Text, Icon, Circle } from "@chakra-ui/react";
import { FaBell } from "react-icons/fa";

export const NotificationBadge = ({
  count,
  counterBg = "red.500",
  iconBg = "gray.600",
  icon = FaBell,
  ...p
}) => {
  return (
    <IconButton
      isRound
      variant="ghost"
      aria-label="Notifications"
      size="lg"
      css={{
        position: "relative !important",
      }}
      icon={
        <>
          <Icon as={icon} color={iconBg} />
          <Circle
            size="22px"
            bg={counterBg}
            color="white"
            position="absolute"
            top="6px"
            right="1px"
            borderColor="white"
            borderWidth={2}
          >
            <Text fontSize="xs">{count}</Text>
          </Circle>
        </>
      }
      {...p}
    />
  );
};
