import * as React from "react";

import { Box, Container, Heading, Text, VStack, chakra, useColorModeValue } from "@chakra-ui/react";

import Navbar from "../components/layout/Navbar";
import { SlideFade } from "@chakra-ui/react";

const Home = () => {
  return (
    <Box>
      <Navbar />
      <Container maxW="container.xl" centerContent py={4}>
        <VStack w={"full"} spacing={4}>
          <SlideFade offsetY="-20px" in>
            <Heading>Heading</Heading>
          </SlideFade>

          <Box bg={useColorModeValue("gray.50", "gray.800")}>
            <Box
              maxW="7xl"
              w={{ md: "3xl", lg: "4xl" }}
              mx="auto"
              py={{ base: 12, lg: 16 }}
              px={{ base: 4, lg: 8 }}
              display={{ lg: "flex" }}
              alignItems={{ lg: "center" }}
              justifyContent={{ lg: "space-between" }}
            >
              <Text
                fontSize={{ base: "3xl", sm: "4xl" }}
                fontWeight="extrabold"
                letterSpacing="tight"
                lineHeight="shorter"
                color={useColorModeValue("gray.900", "gray.100")}
              >
                <chakra.span display="block">Ready to dive in?</chakra.span>
                <chakra.span display="block" color={useColorModeValue("core.pink.600", "gray.500")}>
                  Add your components here
                </chakra.span>
              </Text>
            </Box>
          </Box>
        </VStack>
      </Container>
    </Box>
  );
};

export default Home;
