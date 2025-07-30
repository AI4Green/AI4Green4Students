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

export const Home = () => {
  const { t } = useTranslation();

  return (
    <VStack align="stretch">
      <Container py={4} mx="auto" px={8}>
        <VStack spacing={16} textAlign="center" py={20}>
          <VStack spacing={6}>
            <Heading
              fontWeight="normal"
              fontSize={{ base: "4xl", md: "6xl" }}
              color="brand.500"
              letterSpacing="tighter"
            >
              {t("brand.name")}
            </Heading>
            <Heading
              fontSize={{ base: "2xl", md: "4xl" }}
              fontWeight="light"
              maxW="xl"
            >
              {t("brand.heading")}
            </Heading>
            <Text fontSize="xl" color="gray.600">
              {t("brand.subheading")}
            </Text>
          </VStack>

          <VStack spacing={12}>
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

            <HStack spacing={2}>
              {navbarItems.map((item, index) => (
                <MotionButton
                  as={Link}
                  to={item.href}
                  key={item.label}
                  leftIcon={<item.icon />}
                  variant="ghost"
                  size="md"
                  initial={{ opacity: 0, y: 20 }}
                  animate={{ opacity: 1, y: 0 }}
                  transition={{ duration: 0.5, delay: index * 0.1 }}
                >
                  {item.label}
                </MotionButton>
              ))}
            </HStack>
          </VStack>
        </VStack>

        <VStack spacing={8} mb={20}>
          <Heading fontSize="3xl" fontWeight="light">
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
                borderRadius="md"
                border="1px solid"
                borderColor="gray.200"
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

        <VStack spacing={4} textAlign="center">
          <Image
            src="/assets/RAE_Logo_Black_RGB.png"
            alt="Royal Academy of Engineering Logo"
            maxW="200px"
            mx="auto"
          />
          <Heading fontSize="sm" fontWeight="light">
            Supported by the grant from the Royal Academy of Engineering.
          </Heading>
        </VStack>
      </Container>
      <Footer />
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
