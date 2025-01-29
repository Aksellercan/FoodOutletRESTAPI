-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Generation Time: Jan 29, 2025 at 01:01 PM
-- Server version: 10.4.32-MariaDB
-- PHP Version: 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `foodoutletdb`
--

-- --------------------------------------------------------

--
-- Table structure for table `foodoutlets`
--

CREATE TABLE `foodoutlets` (
  `Id` int(11) NOT NULL,
  `Name` varchar(255) NOT NULL,
  `Location` varchar(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `foodoutlets`
--

INSERT INTO `foodoutlets` (`Id`, `Name`, `Location`) VALUES
(8, 'Kebab King 4', 'poznan'),
(10, 'Rover', 'mars'),
(11, 'Pizza Hut', 'US New york'),
(12, 'KFC', 'US Kentucky'),
(14, 'Stary Browar MC', 'poznan'),
(15, 'Cafe', 'Poznan PL'),
(16, 'Cafe the sequel', 'Poznan PL'),
(18, 'nirvana', 'Seattle USA'),
(19, 'Serve the Servants', 'Seattle USA'),
(21, 'Blur', 'England London'),
(23, 'sametin yeri', '342 Stary Rynek Poznan');

-- --------------------------------------------------------

--
-- Table structure for table `reviews`
--

CREATE TABLE `reviews` (
  `Id` int(11) NOT NULL,
  `FoodOutletId` int(11) NOT NULL,
  `Comment` text DEFAULT NULL,
  `Score` int(11) NOT NULL CHECK (`Score` between 1 and 5),
  `CreatedAt` datetime NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `reviews`
--

INSERT INTO `reviews` (`Id`, `FoodOutletId`, `Comment`, `Score`, `CreatedAt`) VALUES
(8, 10, 'admin review', 5, '2025-01-26 17:07:18'),
(9, 10, 'admin reviews 3', 5, '2025-01-26 20:32:36'),
(10, 8, 'Postman Reviews i have an idea!', 3, '2025-01-28 01:33:40'),
(11, 10, 'admin reviews 3', 2, '2025-01-28 16:18:48'),
(12, 8, 'testing sending review from the spa', 4, '2025-01-28 18:45:26'),
(13, 8, 'this should post', 3, '2025-01-28 18:46:43'),
(14, 10, 'more reviews', 1, '2025-01-28 19:03:20'),
(15, 10, 'another one', 1, '2025-01-28 19:03:27'),
(16, 10, 'more\n', 1, '2025-01-28 19:03:33'),
(17, 10, 'Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry\'s standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.', 5, '2025-01-28 19:03:58'),
(18, 10, 'testing new status code', 4, '2025-01-28 19:16:59'),
(19, 10, 'jjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjk', 1, '2025-01-28 19:19:01'),
(20, 10, 'ggggggggggggwwgg', 4, '2025-01-28 19:22:32'),
(23, 8, 'should i post', 3, '2025-01-28 19:34:54'),
(24, 12, 'wfwfw', 1, '2025-01-28 19:44:45'),
(25, 12, 'test ing 11241', 5, '2025-01-28 19:44:59'),
(26, 14, 'wont stand down', 5, '2025-01-28 19:46:11'),
(27, 8, 'test', 1, '2025-01-28 21:18:46'),
(28, 15, 'Good coffee', 5, '2025-01-28 21:19:03'),
(29, 15, 'Mountains ', 4, '2025-01-28 21:19:28'),
(30, 18, 'Francis farmer will have her revenge', 5, '2025-01-28 21:39:31'),
(31, 19, 'Nirvana', 5, '2025-01-29 00:21:58'),
(32, 19, 'another one', 1, '2025-01-29 00:22:20'),
(33, 8, 'admin reviews 3', 2, '2025-01-29 02:03:32'),
(34, 8, 'Test method review', 5, '2025-01-29 02:04:13'),
(35, 8, 'Test method review', 5, '2025-01-29 02:17:26'),
(36, 8, 'Test method review', 5, '2025-01-29 02:38:19'),
(37, 8, 'Test method review', 5, '2025-01-29 02:40:20'),
(38, 14, 'Clock strikes midnight', 4, '2025-01-29 02:54:03'),
(39, 11, 'Great Pizza!', 5, '2025-01-29 03:01:18'),
(40, 11, 'Terrible Pizza!', 1, '2025-01-29 03:01:31'),
(41, 16, 'post reviews', 4, '2025-01-29 03:12:26'),
(42, 21, 'great', 3, '2025-01-29 03:23:28'),
(43, 21, 'acting', 1, '2025-01-29 03:25:17'),
(45, 21, 'good', 4, '2025-01-29 12:00:10'),
(46, 12, 'good', 1, '2025-01-29 12:00:27');

-- --------------------------------------------------------

--
-- Table structure for table `users`
--

CREATE TABLE `users` (
  `Id` int(11) NOT NULL,
  `Username` varchar(255) NOT NULL,
  `Password` varchar(255) NOT NULL,
  `Role` varchar(50) NOT NULL DEFAULT 'User'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `users`
--

INSERT INTO `users` (`Id`, `Username`, `Password`, `Role`) VALUES
(1, 'testuser', 'password123', 'User'),
(2, 'testuser2', 'password123', 'User'),
(3, 'aksell', '123456', 'User'),
(4, 'admin', 'admin', 'Admin'),
(5, 'Frontendtest', '123456', 'User'),
(6, 'lleska', '654321', 'User'),
(7, 'newuser', '123', 'User'),
(8, 'samet', '12345', 'User');

--
-- Indexes for dumped tables
--

--
-- Indexes for table `foodoutlets`
--
ALTER TABLE `foodoutlets`
  ADD PRIMARY KEY (`Id`);

--
-- Indexes for table `reviews`
--
ALTER TABLE `reviews`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `FoodOutletId` (`FoodOutletId`);

--
-- Indexes for table `users`
--
ALTER TABLE `users`
  ADD PRIMARY KEY (`Id`),
  ADD UNIQUE KEY `Username` (`Username`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `foodoutlets`
--
ALTER TABLE `foodoutlets`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=24;

--
-- AUTO_INCREMENT for table `reviews`
--
ALTER TABLE `reviews`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=47;

--
-- AUTO_INCREMENT for table `users`
--
ALTER TABLE `users`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=9;

--
-- Constraints for dumped tables
--

--
-- Constraints for table `reviews`
--
ALTER TABLE `reviews`
  ADD CONSTRAINT `reviews_ibfk_1` FOREIGN KEY (`FoodOutletId`) REFERENCES `foodoutlets` (`Id`) ON DELETE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
