import * as React from "react";

import { Avatar, Badge, Box, Button, Flex, Image, Menu, MenuButton, MenuItem, MenuList, Text, Tooltip } from "@chakra-ui/react";
import { useAppDispatch, useAppSelector } from "../../shared/hooks";

import logo from "../../assets/images/React-icon.svg";
import { thunkLogout } from "../../redux/slices/authSlice";
import { useHistory } from "react-router-dom";

//import Swal from "sweetalert2";
//import { client } from "../../shared/services";

const Navbar = () => {
  const { NODE_ENV } = process.env;
  const dispatch = useAppDispatch();
  const history = useHistory();
  const user = useAppSelector((state) => state.auth.user);
  return (
    <>
      <Box boxShadow="md" bg="core.gray.medium" py={2} px={4} w="full">
        <Flex h={[32, 16]} alignItems={"center"} justifyContent={"space-between"} direction={["column", "row"]}>
          <Image boxSize="150px" src={logo} alt="logo" marginBottom={2} marginLeft={2} />

          <Flex alignItems={"center"}>
            {NODE_ENV === "development" ? (
              <>
                <Badge
                  variant="outline"
                  as={Button}
                  onClick={() => window.open("/swagger", "_blank")}
                  mr="3"
                  fontSize="1.3em"
                  colorScheme="green"
                  px={4}
                >
                  Open Swagger
                </Badge>
                <Tooltip label="Make sure you have Seq installed locally">
                  <Badge
                    variant="outline"
                    as={Button}
                    onClick={() => window.open("http://localhost:5341/", "_blank")}
                    mr="3"
                    fontSize="1.3em"
                    colorScheme="core.cyan"
                    px={4}
                  >
                    View Logs
                  </Badge>
                </Tooltip>
              </>
            ) : null}
            {NODE_ENV === "production" ? (
              <Badge mr="3" fontSize="1.3em" colorScheme="blue">
                Production (v0.41)
              </Badge>
            ) : null}

            <Text display={{ base: "none", md: "flex" }} fontWeight="bold" fontSize="lg" mr={2}>
              Welcome, {user?.name}
            </Text>
            <Menu>
              <MenuButton as={Button} rounded={"full"} variant={"link"} cursor={"pointer"}>
                <Avatar fontWeight="bold" fontSize="xl" size={"md"} name={user?.name} />
              </MenuButton>
              <MenuList>
                <MenuItem
                  onClick={() => {
                    dispatch(thunkLogout(history));
                  }}
                >
                  Logout
                </MenuItem>
              </MenuList>
            </Menu>
          </Flex>
        </Flex>
      </Box>
    </>
  );
};

export default Navbar;
