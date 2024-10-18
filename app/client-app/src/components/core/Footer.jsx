import {
  Box,
  Center,
  Divider,
  Flex,
  HStack,
  Image,
  Link,
  Stack,
  Text,
} from "@chakra-ui/react";
import { Fragment } from "react";
import { Link as RouterLink } from "react-router-dom";

const isLocalUrl = (url) => url.startsWith("/");

const FooterLink = ({ children, url }) => {
  const linkProps = isLocalUrl(url)
    ? { to: url, as: RouterLink }
    : { href: url, isExternal: true };
  return (
    <Link {...linkProps}>
      <Text fontSize={{ base: "xs", md: "sm" }} color="gray.300">
        {children}
      </Text>
    </Link>
  );
};

const SmallFooter = ({ copyrightText, links }) => (
  <Flex justify="space-between" px={2}>
    {copyrightText && <Text>{copyrightText}</Text>}

    <HStack>
      {links.map((group, i) => (
        <Fragment key={i}>
          {i !== 0 && (
            <Center height="100%">
              <Divider orientation="vertical" />
            </Center>
          )}
          {Object.keys(group).map((k, i) => (
            <Fragment key={i}>
              {i !== 0 && (
                <Center height="100%">
                  <Divider orientation="vertical" />
                </Center>
              )}
              <FooterLink key={i} url={group[k]}>
                {k}
              </FooterLink>
            </Fragment>
          ))}
        </Fragment>
      ))}
    </HStack>
  </Flex>
);

const BigFooter = ({ copyrightText, links }) => (
  <HStack
    bgGradient="radial(circle 400px at top left, cyan.600, blue.900)"
    align="stretch"
    justify="center"
    p={1}
    spacing={4}
  >
    {copyrightText && (
      <Flex w="100%" justify="center">
        <Text>{copyrightText}</Text>
      </Flex>
    )}
    {/* Logos */}
    <HStack justify="space-evenly" p={2}>
      <Image
        h="55px"
        src="/assets/uon_rgb_trans.png"
        alt="University of Nottingham logo"
      />
    </HStack>
    {/* Links */}
    <HStack justify="space-evenly">
      {links.map((group, i) => (
        <Stack
          key={i}
          direction={{ base: "column", md: "row" }}
          spacing={{ base: 0, md: 2 }}
        >
          {Object.keys(group).map((k, i) => (
            <Fragment key={i}>
              {i !== 0 && (
                <Box height={4} display={{ base: "none", md: "block" }}>
                  <Divider
                    orientation="vertical"
                    borderColor="white"
                    height="100%"
                  />
                </Box>
              )}
              <FooterLink key={i} url={group[k]}>
                {k}
              </FooterLink>
            </Fragment>
          ))}
        </Stack>
      ))}
    </HStack>
  </HStack>
);

export const Footer = ({ isSmall }) => {
  const copyrightText = `© ${new Date().getFullYear()} AI4Green4Students`;

  const footerLinks = [
    {
      About: "/about",
      "Green Chemistry": "/greenchemistry",
      "Sustainability Metrics": "/metrics",
    },
  ];

  return isSmall ? (
    <SmallFooter links={footerLinks} copyrightText={null} />
  ) : (
    <BigFooter links={footerLinks} copyrightText={null} />
  );
};
