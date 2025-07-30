import {
  VStack,
  Heading,
  Button,
  Container,
  HStack,
  SimpleGrid,
  Box,
  Text,
  Icon,
  Image,
  Wrap,
  WrapItem,
  useBreakpointValue,
} from "@chakra-ui/react";
import { useTranslation } from "react-i18next";
import { FaFlask, FaUsers, FaChartLine, FaLeaf } from "react-icons/fa";
import { GiNotebook } from "react-icons/gi";
import { motion } from "framer-motion";
import { Footer } from "components/core/Footer";
import { navbarItems } from "config/navbar-items";
import { Link } from "react-router-dom";

const MotionBox = motion(Box);
const MotionButton = motion(Button);
const MotionHeading = motion(Heading);
const MotionText = motion(Text);

export const Home = () => {
  const { t } = useTranslation();

  return (
    <VStack
      spacing={4}
      align="stretch"
      minHeight="100vh"
      position="relative"
      bgGradient="linear(135deg, green.50 0%, blue.50 50%, purple.50 100%)"
      _before={{
        content: '""',
        position: "absolute",
        top: 0,
        left: 0,
        right: 0,
        bottom: 0,
        bgGradient: "linear(135deg, green.50 0%, blue.50 50%, purple.50 100%)",
        animation: "gradientShift 8s ease-in-out infinite",
        zIndex: 0,
      }}
      sx={{
        "@keyframes gradientShift": {
          "0%, 100%": {
            background:
              "linear-gradient(135deg, var(--chakra-colors-green-50) 0%, var(--chakra-colors-blue-50) 50%, var(--chakra-colors-purple-50) 100%)",
          },
          "50%": {
            background:
              "linear-gradient(135deg, var(--chakra-colors-blue-50) 0%, var(--chakra-colors-purple-50) 50%, var(--chakra-colors-green-50) 100%)",
          },
        },
      }}
    >
      <Box
        height="60vh"
        display="flex"
        alignItems="center"
        justifyContent="center"
        position="relative"
        zIndex={1}
      >
        <Container py={4} mx="auto" px={8}>
          <VStack spacing={20} textAlign="center">
            <VStack spacing={6}>
              <MotionHeading
                fontWeight="bold"
                fontSize={{
                  base: "4xl",
                  sm: "5xl",
                  md: "6xl",
                  xl: "8xl",
                }}
                color="brand.500"
                letterSpacing="tighter"
                position="relative"
                initial={{ opacity: 0, y: 30 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{
                  duration: 0.8,
                  ease: "easeOut",
                  delay: 0.2,
                }}
              >
                {t("brand.name")}
              </MotionHeading>
              <MotionHeading
                fontSize={{ base: "2xl", md: "5xl" }}
                letterSpacing="tighter"
                fontWeight="semibold"
                maxW={{ base: "full", sm: "lg", md: "2xl" }}
                initial={{ opacity: 0, y: 20 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{
                  duration: 0.6,
                  ease: "easeOut",
                  delay: 0.6,
                }}
              >
                {t("brand.heading")}
              </MotionHeading>
              <MotionText
                fontSize="xl"
                color="gray.600"
                initial={{ opacity: 0, y: 15 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{
                  duration: 0.5,
                  ease: "easeOut",
                  delay: 1.0,
                }}
              >
                {t("brand.subheading")}
              </MotionText>
            </VStack>

            <VStack spacing={20}>
              <MotionButton
                as={Link}
                to="account/login"
                size="lg"
                colorScheme="green"
                borderRadius="full"
                px={12}
                py={6}
                fontSize="lg"
                position="relative"
                overflow="hidden"
                whileHover={{
                  scale: 1.05,
                  y: -2,
                }}
                whileTap={{ scale: 0.95 }}
                _before={{
                  content: '""',
                  position: "absolute",
                  top: 0,
                  left: "-100%",
                  width: "100%",
                  height: "100%",
                  background:
                    "linear-gradient(90deg, transparent, rgba(255,255,255,0.2), transparent)",
                  borderRadius: "full",
                  animation: "shimmer 2s infinite",
                  zIndex: 1,
                }}
                sx={{
                  "@keyframes shimmer": {
                    "0%": { left: "-100%" },
                    "100%": { left: "100%" },
                  },
                }}
              >
                Get Started
              </MotionButton>

              <Wrap spacing={2} justify="center">
                {navbarItems.map((item, index) => (
                  <WrapItem key={item.label}>
                    <MotionButton
                      as={Link}
                      to={item.href}
                      leftIcon={<item.icon />}
                      variant="ghost"
                      size={useBreakpointValue({ base: "sm", md: "md" })}
                      animate={{
                        opacity: [1, 0, 1],
                        y: [0, 20, 0],
                      }}
                      transition={{
                        duration: 0.6,
                        delay: index * 0.1,
                        repeat: Infinity,
                        repeatDelay: 20,
                        repeatType: "loop",
                      }}
                    >
                      {item.label}
                    </MotionButton>
                  </WrapItem>
                ))}
              </Wrap>
            </VStack>
          </VStack>
        </Container>
      </Box>

      <Container
        py={4}
        mx="auto"
        px={8}
        position="relative"
        zIndex={1}
        flex="1"
      >
        <VStack spacing={8} mb={20}>
          <Heading fontSize="3xl" fontWeight="medium">
            Key Features
          </Heading>
          <SimpleGrid columns={{ base: 1, md: 2, lg: 3 }} spacing={8} w="full">
            {features.map((feature, index) => (
              <MotionBox
                key={index}
                initial={{ opacity: 0, y: 50 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ duration: 0.6, delay: index * 0.1 }}
                p={6}
              >
                <Icon as={feature.icon} w={6} h={6} color="brand.500" mb={4} />
                <Heading size="md" mb={3}>
                  {feature.title}
                </Heading>
                <Text color="gray.600" fontSize="sm">
                  {feature.description}
                </Text>
              </MotionBox>
            ))}
          </SimpleGrid>
        </VStack>
      </Container>

      <HStack spacing={4} textAlign="center" zIndex={1} justify="center">
        {logos.map((logo, index) => (
          <VStack key={index} spacing={4} align="center">
            <Image
              src={logo.src}
              alt={logo.alt}
              maxW="200px"
              objectFit="contain"
            />
            <Text fontSize="xs" color="gray.600">
              {logo.text}
            </Text>
          </VStack>
        ))}
      </HStack>

      <Box position="relative" zIndex={1} mt="auto">
        <Footer />
      </Box>
    </VStack>
  );
};

const features = [
  {
    icon: FaFlask,
    title: "Sustainable Chemistry Learning",
    description:
      "Interactive modules focused on sustainable chemistry practices",
  },
  {
    icon: GiNotebook,
    title: "Electronic Laboratory Notebook",
    description: "Digital tools for organising and managing research data",
  },
  {
    icon: FaUsers,
    title: "Collaboration Tools",
    description: "Work seamlessly with peers and mentors",
  },
  {
    icon: FaChartLine,
    title: "Green Metrics Calculator",
    description: "Quantify the greenness of your reactions",
  },
  {
    icon: FaLeaf,
    title: "Real-time Feedback",
    description: "Immediate correction of misconceptions",
  },
];

const logos = [
  {
    src: "/assets/RAE_Logo_Black_RGB.png",
    alt: "Royal Academy of Engineering Logo",
    text: "Supported by the grant from the Royal Academy of Engineering.",
  },
];
