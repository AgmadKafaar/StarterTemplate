import * as React from "react";

import {
  Box,
  Button,
  Checkbox,
  Container,
  Flex,
  FormControl,
  FormLabel,
  HStack,
  Heading,
  Image,
  Input,
  Link,
  Stack,
  Text,
} from "@chakra-ui/react";
import { useAppDispatch, useAppSelector } from "./../shared/hooks";

import logo from "../../assets/images/React-icon.svg";
import { thunkLogin } from "../redux/slices/authSlice";
import { useForm } from "react-hook-form";
import { useHistory } from "react-router-dom";

export default function SimpleCard() {
  const history = useHistory();
  const dispatch = useAppDispatch();
  const isLoading = useAppSelector((state) => state.auth.isLoading);

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm();
  const onSubmit = (data: any) => {
    console.log(data);
    dispatch(thunkLogin(data.username, data.password, history));
  };

  return (
    <Container bg="core.gray.dark" h="100vh">
      <Flex direction="column" justifyContent="center" h="100%">
        <Stack spacing={8}>
          <Image src={logo} alt="logo" px={16} />
          <Box rounded={"lg"} bg="core.gray.medium" boxShadow={"lg"} p={8} align={"center"}>
            <Heading fontSize={"4xl"} paddingBottom={8}>
              Sign in
            </Heading>
            <form onSubmit={handleSubmit(onSubmit)}>
              <Flex direction="column" spacing={4}>
                <FormControl id="username">
                  <FormLabel>Username</FormLabel>
                  <Input
                    focusBorderColor="core.pink.main"
                    {...register("username", { required: true })}
                    type="text"
                    size="lg"
                  />
                  {errors.username && (
                    <Text align="start" color={"core.flame.main"}>
                      *Username required
                    </Text>
                  )}
                </FormControl>
                <FormControl my="4" id="password">
                  <FormLabel>Password</FormLabel>
                  <Input
                    focusBorderColor="core.pink.main"
                    {...register("password", { required: true })}
                    type="password"
                    size="lg"
                  />
                  {errors.password && (
                    <Text align="start" color={"core.flame.main"}>
                      *Password required
                    </Text>
                  )}
                </FormControl>

                <HStack align={"start"} justify={"space-between"}>
                  <Checkbox>Remember me</Checkbox>
                  <Link color={"blue.400"}>Forgot password?</Link>
                </HStack>
                <Button
                  isLoading={isLoading}
                  loadingText="Signing In"
                  mt="12"
                  type="submit"
                  size="lg"
                  width="full"
                  border="2px"
                  borderColor="core.green.main"
                >
                  Sign in
                </Button>
              </Flex>
            </form>
          </Box>
        </Stack>
      </Flex>
    </Container>
  );
}
